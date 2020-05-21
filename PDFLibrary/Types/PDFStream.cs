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

        public override void Write(Stream output)
        {
            MemoryStream streamData;
            string filterName;
            int length;

            switch (filter)
            {
                case Filter.Flate:
                    /*
                    // TODO Flate compression does not work yet
                    streamData = new MemoryStream(1000);
                    filterName = "FlateDecode";
                    using (Stream deflateStream = new DeflateStream(streamData, CompressionMode.Compress))
                    {
                        Data.WriteTo(deflateStream);
                        length = (int)streamData.Length; // get length before streamData is closed
                    }
                    break;
                    */
                case Filter.None:
                    streamData = Data;
                    filterName = null;
                    length = (int)Data.Length;
                    break;
                default:
                    throw new Exception("Invalid stream filter");
            }

            StreamDictionary.Put("Length", new PDFInt(length));
            if (filterName != null)
            {
                StreamDictionary.Put("Filter", new PDFName(filterName));
            }

            WriteASCIIBytes($"{ObjectNumber} 0 obj\r\n{StreamDictionary.ToString()}\r\nstream\r\n", output);
            streamData.WriteTo(output);
            WriteASCIIBytes("\r\nendstream\r\nendobj\r\n\r\n", output);
        }
    }
}
