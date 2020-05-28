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

        private IGraphicsContext _gc;
        private Menu _menu;
        private List<List<Dish>> _dishes;
        private Brush _themeColorBrush = Brushes.Red;
        private Brush _pageBackgroundBrush;
        private FontFamily _fontFamily = new FontFamily("Arial");
        private List<MenuPage> _menuPages = new List<MenuPage>();
        private Typeface _plainFont;
        private Typeface _boldFont;

        public MenuGraphicsCreator()
        {

        }

        public void LoadMenu(int menuId)
        {
            try
            {
                DataAccess.DataAccess da = new DataAccess.DataAccess();
                _menu = da.GetMenuById(menuId); 
                _dishes = da.GetAllDishesInAllCategories(menuId);
                _menuPages = MenuPage.CreatePages(_dishes);
            }
            catch (Exception)
            {

            }
        }

        public int PageCount 
        { 
            get
            {
                return _menuPages.Count;
            }
        }

        public void Start(IGraphicsContext gc, System.Windows.Media.Color themeColor, 
            Brush pageBackground, FontFamily family)
        {
            this._fontFamily = (family == null) ? new FontFamily("Arial") : family;
            _plainFont = new Typeface(_fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            _boldFont = new Typeface(_fontFamily, FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
            this._gc = gc;
            this._pageBackgroundBrush = pageBackground;
            _themeColorBrush = new SolidColorBrush(themeColor);
        }

        public void End()
        {
            this._gc = null;
        }

        public void DrawAllMenuPages(bool showBorder, bool showOrnaments)
        {
            for (int i = 0; i < _menuPages.Count; i++)
            {
                DrawMenuPage(i, showBorder, showOrnaments);
            }
        }

        public void DrawMenuPage(int pageIndex, bool showBorder, bool showOrnaments)
        {
            _gc.StartPage();

            try
            {
                if (_pageBackgroundBrush != null)
                {
                    _gc.DrawRectangle(0, 0, PageWidth, PageHeight, Brushes.Transparent, _pageBackgroundBrush, 1.0);
                }
                if (_fontFamily == null)
                {
                    _fontFamily = new FontFamily("Arial");
                }

                Typeface plainFont = new Typeface(_fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                Typeface boldFont = new Typeface(_fontFamily, FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);

                if (_menu == null)
                {
                    _gc.DrawText("No Menu", boldFont, 36.0, _themeColorBrush, 0, 75.0, true);
                    return;
                }
                if (pageIndex < 0 || pageIndex >= _menuPages.Count)
                {
                    _gc.DrawText($"Page {pageIndex + 1} does not exist", boldFont, 36.0, _themeColorBrush, 0, 75.0, true);
                    return;
                }

                if (showBorder)
                {
                    _gc.DrawRectangle(20, 20, PageWidth - 40, PageHeight - 40, _themeColorBrush, Brushes.Transparent, 4.0);
                    _gc.DrawRectangle(30, 30, PageWidth - 60, PageHeight - 60, _themeColorBrush, Brushes.Transparent, 1.0);
                }
                if (showOrnaments)
                {
                    _gc.DrawCurve(45, 52, 114, 18, 195, 105, 236, 48, _themeColorBrush, 2.0);
                    _gc.DrawCurve(549, 52, 480, 18, 399, 105, 358, 48, _themeColorBrush, 2.0);
                    _gc.DrawCurve(45, 782, 114, 748, 195, 835, 236, 778, _themeColorBrush, 2.0);
                    _gc.DrawCurve(549, 782, 480, 748, 399, 835, 358, 778, _themeColorBrush, 2.0);
                }

                if (pageIndex == 0)
                {
                    _gc.DrawText(_menu.Name, boldFont, 36.0, _themeColorBrush, 0, 75.0, true);
                    _gc.DrawText(_menu.Description, plainFont, 15.0, Brushes.Black, 0, 115.0, true);
                }

                _menuPages[pageIndex].Draw(_gc, _plainFont, _boldFont, _themeColorBrush);
            }
            finally
            {
                _gc.EndPage();
            }
        }


    }
}
