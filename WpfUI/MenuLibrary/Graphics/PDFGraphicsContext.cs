using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Point = System.Windows.Point;
using Pen = System.Windows.Media.Pen;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using PDFLibrary;

namespace WpfUI.MenuLibrary.Graphics
{
    public class PDFGraphicsContext : IGraphicsContext
    {
        private PDFCreator pdfCreator;


        public PDFGraphicsContext(string filePath)
        {
            pdfCreator = new PDFCreator(filePath);
        }

        public void Finish()
        {
            pdfCreator.Finish();
        }

        public void DrawText(string text, Typeface font, double fontSize,
            Brush brush, double x, double y, bool horCenterOnPage)
        {
            if (horCenterOnPage)
            {
                var formattedText = new FormattedText(text,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Windows.FlowDirection.LeftToRight, font, fontSize, brush, 1.0);
                x = (MenuGraphicsCreator.PageWidth - formattedText.Width) / 2;
            }

            y += fontSize * font.FontFamily.Baseline;

            string fontFamily = ""; // TODO Get font family name
            pdfCreator.DrawText(text, fontFamily, fontSize, GetPDFColor(brush), x, y);
        }

        public void DrawLine(double x0, double y0, double x1, double y1,
            Brush strokeBrush, double lineWidth)
        {
            pdfCreator.DrawLine(x0, y0, x1, y1, GetPDFColor(strokeBrush), lineWidth);
        }

        public void DrawRectangle(double x, double y, double width, double height,
            Brush strokeBrush, Brush fillBrush, double lineWidth)
        {
            pdfCreator.DrawRectangle(x, y, width, height, 
                GetPDFColor(strokeBrush), GetPDFColor(fillBrush), lineWidth);
        }

        public void DrawCurve(double x0, double y0, double x1, double y1, double x2, double y2,
            double x3, double y3, Brush strokeBrush, double lineWidth)
        {
            pdfCreator.DrawCurve(x0, y0, x1, y1, x2, y2, x3, y3, GetPDFColor(strokeBrush), lineWidth);
        }

        private PDFColor GetPDFColor(Brush brush)
        {
            if (brush == Brushes.Transparent || brush == null)
            {
                return null;
            }

            SolidColorBrush solidBrush = brush as SolidColorBrush;

            if (solidBrush != null)
            {
                Color color = solidBrush.Color;
                return new PDFColor(color.R / 255.0, color.G / 255.0, color.B / 255.0);
            }

            return new PDFColor(0, 0, 0); // black
        }
    }
}
