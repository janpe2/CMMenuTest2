using System;
using System.Collections.Generic;
using System.Text;

namespace PDFLibrary.Types
{
    /// <summary>
    /// PDF data type <c>real</c> number.
    /// </summary>
    public class PDFReal : PDFObject
    {
        public double Value { get; }

        public PDFReal(double value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return RealToString(Value);
        }

        public static string RealToString(double value)
        {
            // Decimal separator must be '.'.
            // Exponential notation is not allowed in PDF.
            return value.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
