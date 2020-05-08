using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.PDFLibrary.Types
{
    /// <summary>
    /// PDF data type array.
    /// </summary>
    public class PDFArray : PDFObject
    {
        public List<PDFObject> Array { get; } = new List<PDFObject>();
        public int ObjectNumber { get; }

        public PDFArray(int objNum)
        {
            ObjectNumber = objNum;
        }

        public override string ToString()
        {
            return "[" + string.Join(" ", Array) + "]";
        }

    }
}
