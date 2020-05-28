using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WpfUI.MenuLibrary;
using WpfUI.MenuLibrary.DataAccess;
using WpfUI.MenuLibrary.Graphics;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;
using WpfUI.Models;
using System.Windows.Controls;
using Menu = WpfUI.Models.Menu;

namespace WpfUI.ViewModels
{
    public class PreviewViewModel : Screen
    {
        public List<Menu> Menus { get; set; } = new List<Menu>();

        private MenuGraphicsCreator _graphicsCreator = new MenuGraphicsCreator();
        public MenuGraphicsCreator CurrentGraphicsCreator 
        {
            get
            {
                return _graphicsCreator;
            }
        }

        private int _currentPageIndex;
        public int CurrentPageIndex
        {
            get 
            { 
                return _currentPageIndex; 
            }
            set
            {
                _currentPageIndex = value;
                NotifyOfPropertyChange(() => CurrentPageIndex);
                PageLabelText = $"Page {CurrentPageIndex + 1} of {MenuPagesMaxIndex + 1}";
                NotifyOfPropertyChange(() => PageLabelText);
            }
        }

        public int MenuPagesMaxIndex
        {
            get
            {
                return (_graphicsCreator.PageCount == 0) ? 0 : _graphicsCreator.PageCount - 1;
            }
        }

        public List<string> AllColorNames { get; set; } = LoadColorNames();

        public Color SelectedColor { get; set; } = Colors.MediumBlue;
        public Color SelectedBackgroundColor { get; set; } = Colors.White;

        private string _selectedColorName = "MediumBlue";
        public string SelectedColorName
        {
            get
            {
                return _selectedColorName;
            }
            set
            {
                SelectedColor = ParseColor(value, SelectedColor);
                _selectedColorName = value;
                NotifyOfPropertyChange(() => SelectedColor);
            }
        }

        private FontFamily _selectedFontFamily = new FontFamily("Times New Roman");
        public FontFamily SelectedFontFamily
        {
            get
            {
                return _selectedFontFamily;
            }
            set
            {
                _selectedFontFamily = value;
                NotifyOfPropertyChange(() => SelectedFontFamily);
            }
        }

        public ICollection<FontFamily> AllFontFamilies { get; } = GetSystemFontFamilies();

        private static ICollection<FontFamily> GetSystemFontFamilies()
        {
            List<FontFamily> list = new List<FontFamily>(Fonts.SystemFontFamilies);
            list.Sort((a, b) => { return a.Source.CompareTo(b.Source); });
            return list;
        }

        private string _selectedBackgroundColorName = "White";
        public string SelectedBackgroundColorName
        {
            get
            {
                return _selectedBackgroundColorName;
            }
            set
            {
                SelectedBackgroundColor = ParseColor(value, SelectedBackgroundColor);
                _selectedBackgroundColorName = value;
                NotifyOfPropertyChange(() => SelectedBackgroundColor);
            }
        }

        private Menu _selectedMenu;
        public Menu SelectedMenu
        {
            get
            {
                return _selectedMenu;
            }
            set
            {
                _selectedMenu = value;
                SelectedMenuId = value.Id;
                CurrentPageIndex = 0;
                NotifyOfPropertyChange(() => SelectedMenu);
                NotifyOfPropertyChange(() => SelectedMenuId);
                NotifyOfPropertyChange(() => MenuPagesMaxIndex);
            }
        }

        private bool _showBorderSelected = true;
        public bool ShowBorderSelected
        {
            get
            {
                return _showBorderSelected;
            }
            set
            {
                _showBorderSelected = value;
                NotifyOfPropertyChange(() => ShowBorderSelected);
            }
        }

        private bool _showOrnamentsSelected = true;

        public bool ShowOrnamentsSelected
        {
            get
            {
                return _showOrnamentsSelected;
            }
            set
            {
                _showOrnamentsSelected = value;
                NotifyOfPropertyChange(() => ShowOrnamentsSelected);
            }
        }

        private int _selectedMenuId;
        public int SelectedMenuId
        {
            get 
            { 
                return _selectedMenuId; 
            }
            set
            {
                _selectedMenuId = value;
                _graphicsCreator.LoadMenu(_selectedMenuId);
            }
        }

        public string PageLabelText { get; set; } = "Page 1 of 1";

        public PreviewViewModel()
        {
            DataAccess da = new DataAccess();
            List<Menu> allMenus = da.GetAllMenus();
            Menus.AddRange(allMenus);
            SelectedMenu = allMenus[0];
        }

        private static List<string> LoadColorNames()
        {
            PropertyInfo[] properties = typeof(Colors).GetProperties();
            List<string> names = new List<string>();
            foreach (PropertyInfo p in properties)
            {
                names.Add(p.Name);
            }
            return names;
        }

        private Color ParseColor(string str, Color defaultColor)
        {
            try
            {
                return (Color)System.Windows.Media.ColorConverter.ConvertFromString(str);
            }
            catch (Exception)
            {
                return defaultColor;
            }
        }

        public void SavePDF()
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
                dialog.Filter = "PDF Files (*.pdf)|*.pdf";
                dialog.Title = "Save PDF";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;
                if (dialog.ShowDialog() == false)
                {
                    return;
                }

                PDFGraphicsContext pgc = new PDFGraphicsContext(dialog.FileName);

                _graphicsCreator.Start(pgc, SelectedColor,
                    new SolidColorBrush(SelectedBackgroundColor), SelectedFontFamily);
                _graphicsCreator.DrawAllMenuPages(ShowBorderSelected, ShowOrnamentsSelected);
                _graphicsCreator.End();

                pgc.Finish();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save PDF.\n" + ex, "Error");
            }

        }
    }
}
