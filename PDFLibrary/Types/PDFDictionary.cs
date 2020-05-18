using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFLibrary.Types
{
    /// <summary>
    /// PDF object type <c>dictionary</c>.
    /// For efficiency and simplicity, keys are strings instead of PDFName objects.
    /// Keys must be interpreted as ASCII text.
    /// </summary>
    public class PDFDictionary : PDFObject
    {
        public Dictionary<string, PDFObject> Entries { get; } = new Dictionary<string, PDFObject>();
        public int ObjectNumber { get; }
        public override bool IsIndirect
        {
            get { return ObjectNumber > DirectObject; }
        }

        public PDFDictionary(int objNum)
        {
            ObjectNumber = objNum;
        }

        public void Put(string key, PDFObject value)
        {
            // Overwrite the old value if the key already exists.
            // Don't use Add(key, value). It will throw if the key exists.
            Entries[key] = value;
        }

        public PDFObject Get(string key)
        {
            PDFObject value;
            if (Entries.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }

        public override string ToString()
        {
            // Note: This never adds "N 0 obj" and "endobj".

            StringBuilder sb = new StringBuilder();
            sb.Append("<<\r\n");

            // TODO Escape illegal characters in keys

            foreach (string key in Entries.Keys)
            {
                PDFObject value = Entries[key];
                sb.Append('/').Append(key).Append(' ');
                if (value.IsIndirect)
                {
                    sb.Append(value.ToReferenceString()).Append("\r\n");
                }
                else
                {
                    sb.Append(value.ToString()).Append("\r\n");
                }
            }

            sb.Append(">>");
            return sb.ToString();
        }

        public override string ToReferenceString()
        {
            return $"{ObjectNumber} 0 R";
        }

        public override void Write(Stream output)
        {
            string str = ToString();
            if (IsIndirect)
            {
                str = $"{ObjectNumber} 0 obj\r\n{str}\r\nendobj\r\n\r\n";
            }
            WriteASCIIBytes(str, output);
        }

    }
}
