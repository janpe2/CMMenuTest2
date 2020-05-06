using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Point = System.Windows.Point;
using Pen = System.Windows.Media.Pen;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace WpfUI.MenuLibrary.Graphics
{
    public class ScreenGraphicsContext : IGraphicsContext
    {
        private DrawingContext dc;
        private System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

        public ScreenGraphicsContext(DrawingContext dc)
        {
            this.dc = dc;       
        }

        public void DrawText(string text, Typeface font, double fontSize,
            Brush brush, double x, double y, Boolean horizontallyCenterOnPage)
        {
            var formattedText = new FormattedText(text,
                    cultureInfo, FlowDirection.LeftToRight, font, fontSize, brush, 1.0);
            if (horizontallyCenterOnPage)
            {
                x = (MenuGraphicsCreator.PAGE_WIDTH - formattedText.Width) / 2;
            }
            dc.DrawText(formattedText, new Point(x, y));
        }

        public void DrawLine(double x0, double y0, double x1, double y1,
            Brush strokeBrush, double lineWidth)
        {
            dc.DrawLine(new Pen(strokeBrush, lineWidth), new Point(x0, y0), new Point(x1, y1));
        }

        public void DrawCurve(double x0, double y0, double x1, double y1, double x2, double y2,
            double x3, double y3, Brush strokeBrush, double lineWidth)
        {
            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(new Point(x0, y0), false, false);
                ctx.BezierTo(new Point(x1, y1), new Point(x2, y2), 
                    new Point(x3, y3), true, false);
            }
            dc.DrawGeometry(Brushes.Transparent, new Pen(strokeBrush, lineWidth), geometry);
        }

        public void DrawRectangle(double x, double y, double width, double height, Brush strokeBrush,
            Brush fillBrush, double lineWidth)
        {
            if (strokeBrush == null)
            {
                strokeBrush = Brushes.Transparent;
            }
            dc.DrawRectangle(fillBrush, new Pen(strokeBrush, lineWidth), new Rect(x, y, width, height));
        }
    }
}
