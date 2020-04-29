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

        public PreviewViewModel()
        {
            DataAccess da = new DataAccess();
            List<Menu> allMenus = da.GetAllMenus();
            Menus.AddRange(allMenus);
        }
    }
}
