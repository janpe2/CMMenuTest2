using System;
using System.Collections.Generic;
using System.Text;

namespace PDFLibrary.Types
{
    /// <summary>
    /// PDF data type <c>boolean</c>.
    /// </summary>
    public class PDFBoolean : PDFObject
    {
        public bool Value { get; }

        public PDFBoolean(bool value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value ? "true" : "false";
        }
    }
}
