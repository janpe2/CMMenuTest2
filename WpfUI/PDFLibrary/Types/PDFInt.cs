using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.PDFLibrary.Types
{
    /// <summary>
    /// PDF data type integer.
    /// </summary>
    public class PDFInt : PDFObject
    {
        public int Value { get; }

        public PDFInt(int value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
