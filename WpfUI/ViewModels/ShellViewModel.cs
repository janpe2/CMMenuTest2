using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WpfUI.MenuLibrary;
using WpfUI.Models;

namespace WpfUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private MenuManager menuManager = new MenuManager();

        public ShellViewModel()
        {
            menuManager.LoadSampleData();
        }

        public void ShowDishes()
        {
            DishViewModel dvm = new DishViewModel(menuManager);
            ActivateItemAsync(dvm, System.Threading.CancellationToken.None);
        }

        public void ShowMenus()
        {
            
        }

        public void ShowCategories()
        {

        }
    }
}
