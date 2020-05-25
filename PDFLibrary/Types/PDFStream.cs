using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace PDFLibrary.Types
{
    /// <summary>
    /// PDF object type <c>stream</c>.
    /// This is always an indirect object.
    /// </summary>
    public class PDFStream : PDFObject
    {
        public int ObjectNumber { get; }
        public override bool IsIndirect
        {
            get { return true; }
        }

        internal PDFDictionary StreamDictionary { get; } = new PDFDictionary(0);

        public MemoryStream Data { get; } = new MemoryStream(1000);
        private Filter filter;

        public enum Filter
        {
            None,
            Flate
        }

        public PDFStream(int obj, Filter filter)
        {
            ObjectNumber = obj;
            this.filter = filter;
        }

        /// <summary>
        /// Writes a string to the data buffer of this stream.
        /// </summary>
        public void WriteData(string str)
        {
            WriteASCIIBytes(str, Data);
        }

        public override string ToString()
        {
            // Can't represent stream data as a string.
            return "-stream-";
        }

        public override string ToReferenceString()
        {
            return $"{ObjectNumber} 0 R";
        }

        private byte[] GetDeflatedData()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (DeflateStream deflate = new DeflateStream(memoryStream, CompressionMode.Compress))
                {
                    Data.Seek(0, SeekOrigin.Begin);
                    Data.CopyTo(deflate);
                    deflate.Close();
                    return memoryStream.ToArray();
                }
            }
        }

        public override void Write(Stream output)
        {
            string filterName;
            byte[] dataBytes;

            switch (filter)
            {
                case Filter.Flate:
                    //dataBytes = GetDeflatedData();
                    //filterName = "FlateDecode";
                    //break;
                case Filter.None:
                    dataBytes = Data.ToArray();
                    filterName = null;
                    break;
                default:
                    throw new Exception($"Invalid stream filter {filter}");
            }

            StreamDictionary.Put("Length", new PDFInt(dataBytes.Length));
            if (filterName != null)
            {
                StreamDictionary.Put("Filter", new PDFName(filterName));
            }

            WriteASCIIBytes($"{ObjectNumber} 0 obj\r\n{StreamDictionary.ToString()}\r\nstream\r\n", output);
            output.Write(dataBytes, 0, dataBytes.Length);
            WriteASCIIBytes("\r\nendstream\r\nendobj\r\n\r\n", output);
        }

    }
}
