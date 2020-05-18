using System;
using System.Collections.Generic;
using System.Text;

namespace PDFLibrary.Types
{
    /// <summary>
    /// PDF object type <c>boolean</c>.
    /// </summary>
    public class PDFBoolean : PDFObject
    {
        public static PDFBoolean FalseInstance { get; } = new PDFBoolean(false);

        public static PDFBoolean TrueInstance { get; } = new PDFBoolean(true);

        public bool Value { get; }

        private PDFBoolean(bool value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value ? "true" : "false";
        }
    }
}
