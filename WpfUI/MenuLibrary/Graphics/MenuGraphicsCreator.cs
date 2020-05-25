using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WpfUI.Models;

namespace WpfUI.MenuLibrary.Graphics
{
    public class MenuGraphicsCreator
    {
        public const double PageWidth = 595;
        public const double PageHeight = 842;

        private IGraphicsContext gc;
        private Menu menu;
        private List<List<Dish>> dishes;
        private Brush themeColorBrush = Brushes.Red;
        private Brush pageBackground;
        private FontFamily _fontFamily;

        public MenuGraphicsCreator()
        {
            
        }

        public void LoadMenu(int menuId)
        {
            try
            {
                DataAccess.DataAccess da = new DataAccess.DataAccess();
                menu = da.GetMenuById(menuId); 
                dishes = da.GetAllDishesInAllCategories(menuId);
            }
            catch (Exception)
            {

            }
        }

        public void Start(IGraphicsContext gc, System.Windows.Media.Color themeColor, 
            Brush pageBackground, FontFamily family)
        {
            this.gc = gc;
            this.pageBackground = pageBackground;
            themeColorBrush = new SolidColorBrush(themeColor);
            this._fontFamily = family;
        }

        public void End()
        {
            this.gc = null;
        }

        public void DrawMenu(bool showBorder, bool showOrnaments)
        {
            if (pageBackground != null)
            {
                gc.DrawRectangle(20, 20, PageWidth, PageHeight, pageBackground, Brushes.Transparent, 4.0);
            }
            if (_fontFamily == null)
            {
                _fontFamily = new FontFamily("Arial");
            }

            Typeface plainFont = new Typeface(_fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            Typeface boldFont = new Typeface(_fontFamily, FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);

            if (menu == null)
            {
                gc.DrawText("No Menu", boldFont, 36.0, themeColorBrush, 0, 75.0, true);
                return;
            }

            if (showBorder)
            {
                gc.DrawRectangle(20, 20, PageWidth - 40, PageHeight - 40, themeColorBrush, Brushes.Transparent, 4.0);
                gc.DrawRectangle(30, 30, PageWidth - 60, PageHeight - 60, themeColorBrush, Brushes.Transparent, 1.0);
            }
            if (showOrnaments)
            {
                gc.DrawCurve(45, 52, 114, 18, 195, 105, 236, 48, themeColorBrush, 2.0);
                gc.DrawCurve(549, 52, 480, 18, 399, 105, 358, 48, themeColorBrush, 2.0);
                gc.DrawCurve(45, 782, 114, 748, 195, 835, 236, 778, themeColorBrush, 2.0);
                gc.DrawCurve(549, 782, 480, 748, 399, 835, 358, 778, themeColorBrush, 2.0);
            }

            gc.DrawText(menu.Name, boldFont, 36.0, themeColorBrush, 0, 75.0, true);
            gc.DrawText(menu.Description, plainFont, 15.0, Brushes.Black, 0, 115.0, true);
            int category = 0;

            if (dishes != null && dishes.Count > 0)
            {
                double y = 145;
                foreach (List<Dish> dishesInCategory in dishes)
                {
                    if (dishesInCategory.Count > 0)
                    {
                        gc.DrawText($"{(Menu.Category)category}", boldFont, 16.0, Brushes.Black, 55.0, y, false);
                        y += 18;
                        gc.DrawLine(55, y, 530, y, themeColorBrush, 1.0);
                        y += 5;
                        foreach (Dish dish in dishesInCategory)
                        {
                            gc.DrawText($"{dish.Name} {dish.Price}", plainFont, 14.0, Brushes.Black, 75.0, y, false);
                            y += 16;
                            gc.DrawText(dish.Description, plainFont, 12.0, Brushes.Gray, 75.0, y, false);
                            y += 17; 
                        }
                        y += 7;
                    }
                    category++;
                }
            }
        }

    }
}
