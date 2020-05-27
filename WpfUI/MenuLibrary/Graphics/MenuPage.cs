using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using WpfUI.Models;

namespace WpfUI.MenuLibrary.Graphics
{
    internal class MenuPage
    {
        int _pageNumber;
        private List<IContentItem> _content = new List<IContentItem>();

        public MenuPage(int pageNumber)
        {
            this._pageNumber = pageNumber;
        }

        public void Draw(IGraphicsContext gc, Typeface normal, Typeface bold, Brush brush)
        {
            foreach (IContentItem item in _content)
            {
                item.Draw(gc, normal, bold, brush);
            }
        }

        interface IContentItem
        {
            public void Draw(IGraphicsContext gc, Typeface normal, Typeface bold, Brush brush);
        }

        class Text : IContentItem
        {
            Brush brush;
            string text;
            double x;
            double y;
            bool isBold;
            double fontSize;
            bool isCentered;

            public Text(string text, bool isBold, double fontSize, Brush brush, double x, double y, 
                bool isCentered)
            {
                this.brush = brush;
                this.text = text;
                this.x = x;
                this.y = y;
                this.isBold = isBold;
                this.fontSize = fontSize;
                this.isCentered = isCentered;
            }

            public void Draw(IGraphicsContext gc, Typeface normal, Typeface bold, Brush brush)
            {
                gc.DrawText(text, isBold ? bold : normal, fontSize, this.brush, x, y, isCentered);
            }
        }

        class Line : IContentItem
        {
            double x1;
            double y1;
            double x2;
            double y2;
            double lineWidth;

            public Line(double x1, double y1, double x2, double y2, double lineWidth)
            {
                this.x1 = x1;
                this.y1 = y1;
                this.x2 = x2;
                this.y2 = y2;
                this.lineWidth = lineWidth;
            }

            public void Draw(IGraphicsContext gc, Typeface normal, Typeface bold, Brush brush)
            {
                gc.DrawLine(x1, y1, x2, y2, brush, lineWidth);
            }
        }

        internal static List<MenuPage> CreatePages(List<List<Dish>> dishes)
        {
            List<MenuPage> menuPages = new List<MenuPage>();
            if (dishes == null || dishes.Count == 0)
            {
                return menuPages;
            }

            const double pageZeroStartY = 145;
            const double otherPagesStartY = 110;
            const double maxY = 760;

            const double categoryNameDy = 20;
            const double categoryLineDy = 5;
            const double dishNameDy = 16;
            const double dishDescrDy = 17;
            const double categorySeparationDy = 7;
            const double dishTotalHeight = dishNameDy + dishDescrDy + categorySeparationDy;
            
            int category = 0;
            int pageNum = 0;
            double y = pageZeroStartY;
            MenuPage page = new MenuPage(pageNum);
            menuPages.Add(page);

            for (int i = 0; i < dishes.Count; i++)
            {
                List<Dish> dishesInCategory = dishes[i];
                if (dishesInCategory.Count > 0)
                {
                    page._content.Add(new Text($"{(Menu.Category)category}", true, 16.0, Brushes.Black, 55.0, y, false));
                    y += categoryNameDy;
                    page._content.Add(new Line(55, y, 530, y, 1.0));
                    y += categoryLineDy;

                    foreach (Dish dish in dishesInCategory)
                    {
                        if (y + dishTotalHeight > maxY)
                        {
                            page = new MenuPage(pageNum++);
                            menuPages.Add(page);
                            y = otherPagesStartY;
                        }

                        page._content.Add(new Text($"{dish.Name} {dish.Price}", false, 14.0, Brushes.Black, 75.0, y, false));
                        y += dishNameDy;
                        page._content.Add(new Text(dish.Description, false, 12.0, Brushes.Gray, 75.0, y, false));
                        y += dishDescrDy;
                    }

                    y += categorySeparationDy;
                }
                category++;
            }

            return menuPages;
        }
    }
}
