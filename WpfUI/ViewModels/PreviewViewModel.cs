using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;
using WpfUI.MenuLibrary;
using WpfUI.MenuLibrary.DataAccess;
using Color = System.Windows.Media.Color;

namespace WpfUI.ViewModels
{
    public class PreviewViewModel : Screen
    {
        public List<Menu> Menus { get; set; } = new List<Menu>();

        public Color SelectedColor { get; set; } = Colors.Red;

        private string _selectedColorName = "System.Windows.Media.Color Red";
        public string SelectedColorName 
        { 
            get { return _selectedColorName; }
            set
            {
                try
                {
                    _selectedColorName = value;
                    SelectedColor = (Color)System.Windows.Media.ColorConverter.ConvertFromString(
                        value.Replace("System.Windows.Media.Color ", ""));
                }
                catch(Exception)
                {

                }
                NotifyOfPropertyChange(() => SelectedColor);
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

        public void SavePDF()
        {

        }
    }
}
