using System;
using System.Collections.Generic;
using System.Text;

namespace PDFLibrary.Types
{
    /// <summary>
    /// PDF object type <c>null</c>.
    /// </summary>
    public class PDFNull : PDFObject
    {
        public static PDFNull Instance { get; } = new PDFNull();

        private PDFNull()
        {

        }

        public override string ToString()
        {
            return "null";
        }
    }
}
