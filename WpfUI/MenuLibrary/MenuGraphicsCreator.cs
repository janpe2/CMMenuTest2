using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfUI.MenuLibrary
{
    public class MenuGraphicsCreator
    {
        public const double PAGE_WIDTH = 595;
        public const double PAGE_HEIGHT = 842;

        private IGraphicsContext gc;
        private Menu menu;
        private List<List<Dish>> dishes;
        private Brush themeColorBrush = Brushes.Red;
        private Brush pageBackground;

        public MenuGraphicsCreator()
        {
            
        }

        public MenuGraphicsCreator(Menu menu)
        {
            this.menu = menu;
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

        public void Start(IGraphicsContext gc, System.Windows.Media.Color themeColor, Brush pageBackground)
        {
            this.gc = gc;
            this.pageBackground = pageBackground;
            themeColorBrush = new SolidColorBrush(themeColor);
        }

        public void End()
        {
            this.gc = null;
        }

        public void DrawMenu(bool showBorder, bool showOrnaments)
        {
            if (pageBackground != null)
            {
                gc.DrawRectangle(20, 20, PAGE_WIDTH, PAGE_HEIGHT, pageBackground, Brushes.Transparent, 4.0);
            }

            FontFamily fontFamily = new FontFamily("Times New Roman");
            Typeface menuNameFont = new Typeface(fontFamily, FontStyles.Italic, FontWeights.Bold, FontStretches.Normal);
            if (menu == null)
            {
                gc.DrawText("No Menu", menuNameFont, 36.0, themeColorBrush, 0, 75.0, true);
                return;
            }

            Typeface dishNameFont = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            Typeface descrFont = new Typeface(fontFamily, FontStyles.Italic, FontWeights.Normal, FontStretches.Normal);
            Typeface categoryNameFont = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);

            if (showBorder)
            {
                gc.DrawRectangle(20, 20, PAGE_WIDTH - 40, PAGE_HEIGHT - 40, themeColorBrush, Brushes.Transparent, 4.0);
                gc.DrawRectangle(30, 30, PAGE_WIDTH - 60, PAGE_HEIGHT - 60, themeColorBrush, Brushes.Transparent, 1.0);
            }
            if (showOrnaments)
            {
                gc.DrawCurve(45, 52, 114, 18, 195, 105, 236, 48, themeColorBrush, 2.0);
                gc.DrawCurve(549, 52, 480, 18, 399, 105, 358, 48, themeColorBrush, 2.0);
                gc.DrawCurve(45, 782, 114, 748, 195, 835, 236, 778, themeColorBrush, 2.0);
                gc.DrawCurve(549, 782, 480, 748, 399, 835, 358, 778, themeColorBrush, 2.0);
            }

            gc.DrawText(menu.Name, menuNameFont, 36.0, themeColorBrush, 0, 75.0, true);
            gc.DrawText(menu.Description, descrFont, 15.0, Brushes.Black, 0, 115.0, true);
            int category = 0;

            if (dishes != null && dishes.Count > 0)
            {
                double y = 145;
                foreach (List<Dish> dishesInCategory in dishes)
                {
                    if (dishesInCategory.Count > 0)
                    {
                        gc.DrawText($"{(Menu.Category)category}", categoryNameFont, 16.0, Brushes.Black, 55.0, y, false);
                        y += 18;
                        gc.DrawLine(55, y, 530, y, themeColorBrush, 1.0);
                        y += 5;
                        foreach (Dish dish in dishesInCategory)
                        {
                            gc.DrawText($"{dish.Name} {dish.Price} €", dishNameFont, 14.0, Brushes.Black, 75.0, y, false);
                            y += 16;
                            gc.DrawText(dish.Description, descrFont, 12.0, Brushes.Gray, 75.0, y, false);
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
