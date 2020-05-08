using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.PDFLibrary.Types
{
    /// <summary>
    /// Indirect refrence to a PDF object.
    /// </summary>
    public class PDFRef : PDFObject
    {
        public int ReferencedObjectNumber { get; }

        public PDFRef(int obj)
        {
            ReferencedObjectNumber = obj;
        }

        public override string ToString()
        {
            return $"{ReferencedObjectNumber} 0 R";
        }
    }
}
