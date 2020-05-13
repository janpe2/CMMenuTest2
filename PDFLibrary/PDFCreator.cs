using PDFLibrary.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
//using System.Windows.Media.Brush;

namespace PDFLibrary
{
    public class PDFCreator
    {
        private List<PDFObject> indirectObjects = new List<PDFObject>();
        private int objectNumberCounter = 1;
        private int pageCount = 1; // TODO value 1 only for testing
        private PDFDictionary trailerDictionary;
        private PDFStream singlePageContentStream; // TODO remove when we have more than 1 page

        public PDFCreator()
        {
            singlePageContentStream = CreateContentStream(PDFStream.Filter.None);
        }

        private void CreatePDFObjects()
        {
            PDFDictionary testFont = CreateIndirectDictionary();
            testFont.Put("Type", new PDFName("Font"));
            testFont.Put("Subtype", new PDFName("Type1"));
            testFont.Put("BaseFont", new PDFName("Times-Roman"));
            testFont.Put("Encoding", new PDFName("WinAnsiEncoding"));

            PDFDictionary fontResources = CreateIndirectDictionary();
            fontResources.Put("F1", testFont);

            PDFDictionary resources = CreateIndirectDictionary();
            resources.Put("Font", fontResources);

            PDFArray mediaBox = new PDFArray(PDFObject.DirectObject,
                new PDFReal(0.0), new PDFReal(0.0), new PDFReal(595.2756), new PDFReal(841.8898));
            PDFArray pageKidsArray = CreateIndirectArray();

            PDFDictionary pages = CreateIndirectDictionary();
            pages.Put("Type", new PDFName("Pages"));
            pages.Put("Count", new PDFInt(pageCount));
            pages.Put("Kids", pageKidsArray);
            pages.Put("Resources", resources);

            PDFStream[] contentStreams = new PDFStream[pageCount];
            contentStreams[0] = singlePageContentStream; // TODO remove when we have more than 1 page

            for (int i = 0; i < pageCount; i++)
            {
                PDFStream stream = singlePageContentStream; // TODO remove when we have more than 1 page
                //PDFStream stream = CreateContentStream(PDFStream.Filter.None); // TODO use flate

                contentStreams[i] = stream;
                stream.WriteData(GetSampleContentStream()); // TODO This is test data for content stream

                PDFDictionary page = CreateIndirectDictionary();
                pageKidsArray.Array.Add(page);
                page.Put("Type", new PDFName("Page"));
                page.Put("Parent", pages);
                page.Put("Contents", stream);
                page.Put("MediaBox", mediaBox);
                page.Put("Resources", resources); 
            }

            PDFDictionary catalog = CreateIndirectDictionary();
            int objectNumberOfCatalog = catalog.ObjectNumber;
            catalog.Put("Type", new PDFName("Catalog"));
            catalog.Put("Pages", pages);

            PDFString fileIdString = PDFString.CreateFileIDString();

            trailerDictionary = new PDFDictionary(PDFObject.DirectObject);
            trailerDictionary.Put("Size", new PDFInt(indirectObjects.Count + 1));
            trailerDictionary.Put("Root", catalog);
            trailerDictionary.Put("ID", new PDFArray(PDFObject.DirectObject, fileIdString, fileIdString));
        }

        private string GetSampleContentStream()
        {
            return
                "0 0.60 0 RG\r\n" +
                "1 w\r\n" +
                "100 -200 300 100 re S\r\n" +
                "0 0 1 rg\r\n" +
                "BT\r\n" +
                "  /F1 35 Tf\r\n" +
                "  50 -500 Td\r\n" +
                "  (Test string) Tj\r\n" +
                "ET";
        }

        private void WritePDFFile()
        {
            string path = "C:\\Users\\jaa\\Documents\\test1.pdf";
            List<long> xref = new List<long>();

            try
            {
                using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    stream.SetLength(0); // clear previous file content

                    // File header
                    PDFObject.WriteASCIIBytes("%PDF-1.5\r\n", stream);
                    WriteBinaryMarker(stream);

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
            catch (Exception)
            {

                // TODO
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

        private PDFDictionary CreateIndirectDictionary()
        {
            PDFDictionary dict = new PDFDictionary(GetNextObjectNumber());
            indirectObjects.Add(dict);
            return dict;
        }

        private PDFArray CreateIndirectArray()
        {
            PDFArray array = new PDFArray(GetNextObjectNumber());
            indirectObjects.Add(array);
            return array;
        }

        private PDFStream CreateContentStream(PDFStream.Filter filter)
        {
            PDFStream stream = new PDFStream(GetNextObjectNumber(), filter);
            indirectObjects.Add(stream);

            // Each page needs this translate
            stream.WriteData("1 0 0 1 0 841.8898 cm\r\n");

            return stream;
        }

        public void CreatePDF()
        {
            CreatePDFObjects();
            WritePDFFile();
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
            singlePageContentStream.WriteData(str);
        }
    }
}
