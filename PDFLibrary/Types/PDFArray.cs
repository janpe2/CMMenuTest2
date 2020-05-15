using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFLibrary.Types
{
    /// <summary>
    /// PDF data type <c>array</c>.
    /// </summary>
    public class PDFArray : PDFObject
    {
        public List<PDFObject> Array { get; } = new List<PDFObject>();
        public int ObjectNumber { get; }
        public override bool IsIndirect 
        { 
            get { return ObjectNumber > DirectObject; } 
        }

        public PDFArray(int objNum)
        {
            ObjectNumber = objNum;
        }

        public PDFArray(int obj, params PDFObject[] elements)
        {
            this.ObjectNumber = obj;
            this.Array.AddRange(elements);
        }

        public override string ToString()
        {
            // Note: This never adds "N 0 obj" and "endobj".

            StringBuilder sb = new StringBuilder();
            int count = Array.Count;
            sb.Append("[");

            for (int i = 0; i < count; i++)
            {
                PDFObject item = Array[i];
                if (item.IsIndirect)
                {
                    sb.Append(item.ToReferenceString());
                }
                else
                {
                    sb.Append(item.ToString());
                }

                if (i + 1 < count)
                {
                    sb.Append(' ');
                }
            }

            sb.Append("]");
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
