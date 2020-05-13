using PDFLibrary.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFLibrary
{
    /// <summary>
    /// RGB color for PDF. All components are in range 0.0-1.0.
    /// </summary>
    public class PDFColor
    {
        public double Red { get; }

        public double Green { get; }

        public double Blue { get; }

        public PDFColor(double r, double g, double b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        public string ToContentStreamOp(bool isStroke)
        {
            string op = isStroke ? "RG\r\n" : "rg\r\n";
            string r = PDFReal.RealToString(Red);
            string g = PDFReal.RealToString(Green);
            string b = PDFReal.RealToString(Blue);
            return $"{r} {g} {b} {op}";
        }
    }
}
