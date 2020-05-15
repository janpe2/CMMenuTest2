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

namespace WpfUI.ViewModels
{
    public class PreviewViewModel : Screen
    {
        public List<Menu> Menus { get; set; } = new List<Menu>();

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
            get { return _selectedMenu;  } 
            set
            {
                _selectedMenu = value;
                SelectedMenuId = value.Id;
                NotifyOfPropertyChange(() => SelectedMenu);
                NotifyOfPropertyChange(() => SelectedMenuId);
            }
        }

        private bool _showBorderSelected = true;
        public bool ShowBorderSelected
        {
            get { return _showBorderSelected; }
            set 
            {
                _showBorderSelected = value;
                NotifyOfPropertyChange(() => ShowBorderSelected);
            }
        }

        private bool _showOrnamentsSelected = true;
        public bool ShowOrnamentsSelected
        {
            get { return _showOrnamentsSelected; }
            set
            {
                _showOrnamentsSelected = value;
                NotifyOfPropertyChange(() => ShowOrnamentsSelected);
            }
        }

        public int SelectedMenuId { get; set; }

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

                MenuGraphicsCreator graphicsCreator = new MenuGraphicsCreator(SelectedMenu);
                PDFLibrary.PDFCreator pc = new PDFLibrary.PDFCreator(dialog.FileName);
                PDFGraphicsContext pgc = new PDFGraphicsContext(pc);

                pgc.DrawRectangle(0, 0, 595, 842, null, new SolidColorBrush(SelectedBackgroundColor), 1.0);
                //pgc.DrawRectangle(50, 200, 300, 150, new SolidColorBrush(SelectedColor), null, 3.0);

                graphicsCreator.Start(pgc, SelectedColor, null);
                graphicsCreator.DrawMenu(ShowBorderSelected, ShowOrnamentsSelected);
                graphicsCreator.End();

                pc.Finish();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save PDF.\n" + ex, "Error");
            }
            
        }
    }
}
