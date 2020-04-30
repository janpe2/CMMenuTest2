using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUI.MenuLibrary;
using WpfUI.MenuLibrary.DataAccess;

namespace WpfUI.ViewModels
{
    public class PreviewViewModel : Screen
    {
        public List<Menu> Menus { get; set; } = new List<Menu>();

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

        public int SelectedMenuId { get; set; }

        public PreviewViewModel()
        {
            DataAccess da = new DataAccess();
            List<Menu> allMenus = da.GetAllMenus();
            Menus.AddRange(allMenus);
            SelectedMenu = allMenus[0];
        }
    }
}
