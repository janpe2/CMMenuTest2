using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text;

namespace WpfUI.MenuLibrary
{
    public interface IGraphicsContext
    {
        /*
        public const string FONT_TIMES_ROMAN = "Times New Roman";
        public const string FONT_TIMES_ITALIC = "Times New Roman";
        public const string FONT_TIMES_BOLD = "Times New Roman";
        public const string FONT_TIMES_BOLD_ITALIC = "Times New Roman";
        */

        public void DrawText(string text, Typeface font, double fontSize,
            Brush brush, double x, double y, Boolean horCenterOnPage);

        public void DrawLine(double x0, double y0, double x1, double y1,
            Brush strokeBrush, double lineWidth);

        public void DrawRectangle(double x, double y, double width, double height,
            Brush strokeBrush, Brush fillBrush, double lineWidth);

        public void DrawCurve(double x0, double y0, double x1, double y1, double x2, double y2,
            double x3, double y3, Brush strokeBrush, double lineWidth);
    }
}
