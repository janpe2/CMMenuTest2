using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.PDFLibrary.Types
{
    /// <summary>
    /// PDF data type dictionary.
    /// </summary>
    public class PDFDictionary : PDFObject
    {
        public Dictionary<string, PDFObject> Entries { get; } = new Dictionary<string, PDFObject>();
        public int ObjectNumber { get; }

        public PDFDictionary(int objNum)
        {
            ObjectNumber = objNum;
        }

        public void Put(string key, PDFObject value)
        {
            Entries.TryAdd(key, value);
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
            StringBuilder sb = new StringBuilder();
            sb.Append("<<\n");

            // TODO Escape illegal characters of keys

            foreach (string key in Entries.Keys)
            {
                PDFObject value = Entries[key];
                sb.Append('/').Append(key).Append(' ').Append(value.ToString()).Append('\n');
            }

            sb.Append(">>\n");
            return sb.ToString();
        }

    }
}
