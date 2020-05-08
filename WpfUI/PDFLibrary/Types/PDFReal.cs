using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.PDFLibrary.Types
{
    /// <summary>
    /// PDF data type real number.
    /// </summary>
    public class PDFReal : PDFObject
    {
        public double Value { get; }

        public PDFReal(double value)
        {
            this.Value = Value;
        }

        public static string RealToString(double value)
        {
            // Decimal separator must be '.'.
            // Exponential notation is not allowed in PDF.
            return value.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return RealToString(Value);
        }

    }
}
