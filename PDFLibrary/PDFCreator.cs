using PDFLibrary.Font;
using PDFLibrary.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media;
//using System.Windows.Media.Brush;

namespace PDFLibrary
{
    public class PDFCreator : IIndirectObjectCreator
    {
        private List<PDFObject> indirectObjects = new List<PDFObject>();
        private List<PDFDictionary> pageDictionaries = new List<PDFDictionary>();
        private List<PDFStream> pageContentStreams = new List<PDFStream>();
        private int objectNumberCounter = 1;
        private PDFDictionary trailerDictionary;
        private PDFDictionary currentPageDictionary;
        private PDFStream currentContentStream;
        private PDFArray pageKidsArray;
        private PDFDictionary pagesDictionary;
        private PDFDictionary _commonResources;
        private PDFDictionary _commonFontResources;
        private PDFArray mediaBox;
        private string pdfFileName;
        private Dictionary<Typeface, PDFFont> _fontMapping = new Dictionary<Typeface, PDFFont>();

        public PDFCreator(string pdfFileName)
        {
            this.pdfFileName = pdfFileName;

            pageKidsArray = CreateIndirectArray();
            mediaBox = new PDFArray(PDFObject.DirectObject,
                0.0, 0.0, 595.2756, 841.8898); // A4

            pagesDictionary = CreateIndirectDictionary();
            pagesDictionary.Put("Type", new PDFName("Pages"));
            pagesDictionary.Put("Kids", pageKidsArray);
            //pagesDictionary.Put("Resources", ); // all PDF pages would inherit these resources

            PDFDictionary catalog = CreateIndirectDictionary();
            catalog.Put("Type", new PDFName("Catalog"));
            catalog.Put("Pages", pagesDictionary);

            PDFDictionary infoDictionary = CreateIndirectDictionary();
            infoDictionary.Put("Creator", new PDFString("Menu Master"));
            infoDictionary.Put("CreationDate", PDFString.GetDateString());

            PDFString fileIdString = PDFString.CreateFileIDString();

            trailerDictionary = new PDFDictionary(PDFObject.DirectObject);
            trailerDictionary.Put("Root", catalog);
            trailerDictionary.Put("ID", new PDFArray(PDFObject.DirectObject, fileIdString, fileIdString));
            trailerDictionary.Put("Info", infoDictionary);

            _commonResources = CreateIndirectDictionary();
            _commonFontResources = CreateIndirectDictionary();
            _commonResources.Put("Font", _commonFontResources);

            // PDF must have at least one page
            AddPage(); 
        }

        /// <summary>
        /// Adds a new page to the PDF and creates the needed PDF objects.
        /// </summary>
        public void AddPage()
        {
            currentContentStream = CreateContentStream(PDFStream.Filter.None);
            pageContentStreams.Add(currentContentStream);

            currentPageDictionary = CreateIndirectDictionary();
            currentPageDictionary.Put("Type", new PDFName("Page"));
            currentPageDictionary.Put("Parent", pagesDictionary);
            currentPageDictionary.Put("Contents", currentContentStream);
            currentPageDictionary.Put("MediaBox", mediaBox);
            currentPageDictionary.Put("Resources", _commonResources);

            pageDictionaries.Add(currentPageDictionary);
            pageKidsArray.Array.Add(currentPageDictionary);            
        }

        /// <summary>
        /// Finishes PDF creation and writes all PDF objects to the specified file.
        /// This must be called as the final step when creating a PDF.
        /// </summary>
        public void Finish()
        {
            // Set actual page count
            pagesDictionary.Put("Count", new PDFInt(pageDictionaries.Count));
            // Set actual number of objects
            trailerDictionary.Put("Size", new PDFInt(indirectObjects.Count + 1));

            foreach (PDFFont font in _fontMapping.Values)
            {
                font.CreatePDFData(this);
                _commonFontResources.Put($"F{font.ResourceKeyId}", font.GetFontDictionary());
            }

            WritePDFFile();
        }

        private void WritePDFFile()
        {
            List<long> xref = new List<long>();

            using (Stream stream = new FileStream(pdfFileName, FileMode.OpenOrCreate))
            {
                stream.SetLength(0); // clear previous file contents

                // File header
                PDFObject.WriteASCIIBytes("%PDF-1.5\r\n", stream);
                WriteBinaryMarker(stream);

                foreach (PDFFont font in _fontMapping.Values)
                {
                    font.Write(stream);
                }

                foreach (PDFObject obj in indirectObjects)
                {
                    xref.Add(stream.Position);
                    obj.Write(stream);
                }

                long startXref = stream.Position;
                WriteXref(xref, stream);

                PDFObject.WriteASCIIBytes("trailer\r\n", stream);
                trailerDictionary.Write(stream);
                PDFObject.WriteASCIIBytes($"\r\nstartxref\r\n{startXref}\r\n%%EOF\r\n", stream);
            }
        }

        private void WriteBinaryMarker(Stream stream)
        {
            // Binary marker bytes "%âãÏÓ" + "\r\n".
            // Don't use PDFObject.WriteASCIIBytes() because these are not ASCII.
            byte[] binMarker = new byte[]
            {
                (byte)'%', (byte)'â', (byte)'ã', (byte)'Ï', (byte)'Ó', (byte)'\r', (byte)'\n'
            };
            stream.Write(binMarker, 0, binMarker.Length);
        }

        private void WriteXref(List<long> xref, Stream stream)
        {
            PDFObject.WriteASCIIBytes(
                $"xref\r\n0 {indirectObjects.Count + 1}\r\n0000000000 65535 f\r\n", stream);

            foreach (long offset in xref)
            {
                string str = offset.ToString().PadLeft(10, '0');
                PDFObject.WriteASCIIBytes($"{str} 00000 n\r\n", stream);
            }
        }

        private int GetNextObjectNumber()
        {
            return objectNumberCounter++;
        }

        public PDFDictionary CreateIndirectDictionary()
        {
            PDFDictionary dict = new PDFDictionary(GetNextObjectNumber());
            indirectObjects.Add(dict);
            return dict;
        }

        public PDFArray CreateIndirectArray()
        {
            PDFArray array = new PDFArray(GetNextObjectNumber());
            indirectObjects.Add(array);
            return array;
        }

        public PDFArray CreateIndirectArray(params double[] realValues)
        {
            PDFArray array = new PDFArray(GetNextObjectNumber(), realValues);
            indirectObjects.Add(array);
            return array;
        }

        public PDFArray CreateIndirectArray(params int[] intValues)
        {
            PDFArray array = new PDFArray(GetNextObjectNumber(), intValues);
            indirectObjects.Add(array);
            return array;
        }

        public PDFStream CreateStream(PDFStream.Filter filter)
        {
            PDFStream stream = new PDFStream(GetNextObjectNumber(), filter);
            indirectObjects.Add(stream);
            return stream;
        }

        private PDFStream CreateContentStream(PDFStream.Filter filter)
        {
            PDFStream stream = CreateStream(filter);

            // Each page needs this translate
            stream.WriteData("1 0 0 1 0 841.8898 cm\r\n");

            return stream;
        }

        public void DrawRectangle(double x, double y, double width, double height,
            PDFColor stroke, PDFColor fill, double lineWidth)
        {
            WriteColor(fill, false);
            WriteColor(stroke, true);
            WriteLineWidth(stroke, lineWidth);
            AppendToContentStream(
                $"{ PDFReal.RealToString(x) } { PDFReal.RealToString(-y) } { PDFReal.RealToString(width) } { PDFReal.RealToString(-height) } re\r\n");
            WritePaintOp(stroke, fill);
        }

        public void DrawLine(double x0, double y0, double x1, double y1,
            PDFColor stroke, double lineWidth)
        {
            if (stroke == null)
            {
                return;
            }

            WriteColor(stroke, true);
            WriteLineWidth(stroke, lineWidth);
            AppendToContentStream(
                $"{ PDFReal.RealToString(x0) } { PDFReal.RealToString(-y0) } m\r\n" +
                $"{ PDFReal.RealToString(x1) } { PDFReal.RealToString(-y1) } l\r\n");
            AppendToContentStream("S\r\n");
        }

        public void DrawCurve(double x0, double y0, double x1, double y1, double x2, double y2,
            double x3, double y3, PDFColor stroke, double lineWidth)
        {
            // TODO Fill is not yet supported for curve

            if (stroke == null)
            {
                return;
            }

            WriteColor(stroke, true);
            WriteLineWidth(stroke, lineWidth);
            AppendToContentStream(
                $"{ PDFReal.RealToString(x0) } { PDFReal.RealToString(-y0) } m\r\n" +
                $"{ PDFReal.RealToString(x1) } { PDFReal.RealToString(-y1) } " +
                $"{ PDFReal.RealToString(x2) } { PDFReal.RealToString(-y2) } " +
                $"{ PDFReal.RealToString(x3) } { PDFReal.RealToString(-y3) } c\r\n"); 
            AppendToContentStream("S\r\n");
        }

        public void DrawText(string text, Typeface typeface, double fontSize, PDFColor color, 
            double x, double y)
        {
            PDFFont pdfFont;
            if (!_fontMapping.TryGetValue(typeface, out pdfFont))
            {
                pdfFont = new PDFTrueTypeFont(CreateIndirectDictionary(), typeface);
                _fontMapping[typeface] = pdfFont;
                pdfFont.ResourceKeyId = _fontMapping.Count;
            }
            int fontKey = pdfFont.ResourceKeyId;
            pdfFont.AddStringToSubset(text);

            // Let's create a temporary PDFString, which takes care of escaping special characters.
            PDFString str = new PDFString(text);
            WriteColor(color, false);

            AppendToContentStream(
                "BT\r\n" +
                $"  /F{fontKey} {PDFReal.RealToString(fontSize)} Tf\r\n" +
                $"  {PDFReal.RealToString(x)} {PDFReal.RealToString(-y)} Td\r\n" +
                $"  {str.ToString()} Tj\r\n" +
                "ET\r\n");
        }

        private void WriteColor(PDFColor color, bool isStroke)
        {
            if (color != null)
            {
                AppendToContentStream(color.ToContentStreamOp(isStroke));
            }
        }

        private void WriteLineWidth(PDFColor stroke, double lineWidth)
        {
            if (stroke != null)
            {
                AppendToContentStream($"{PDFReal.RealToString(lineWidth)} w\r\n");
            }
        }

        private void WritePaintOp(PDFColor stroke, PDFColor fill)
        {
            if (stroke != null && fill != null)
            {
                AppendToContentStream("B\r\n");
            }
            else if (stroke != null)
            {
                AppendToContentStream("S\r\n");
            }
            else if (fill != null)
            {
                AppendToContentStream("f\r\n");
            }
        }

        private void AppendToContentStream(string str)
        {
            currentContentStream.WriteData(str);
        }
    }
}
