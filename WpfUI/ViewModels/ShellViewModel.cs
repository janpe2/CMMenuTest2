using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WpfUI.MenuLibrary;
using WpfUI.MenuLibrary.DataAccess;
using WpfUI.Models;

namespace WpfUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private MenuManager menuManager = new MenuManager();

        public ShellViewModel()
        {
            menuManager.LoadSampleData();
            ShowDishes();
        }

        public void ShowDishes()
        {
            DishViewModel dvm = new DishViewModel(menuManager);
            ActivateItemAsync(dvm, System.Threading.CancellationToken.None);
        }

        public void ShowMenus()
        {
            CategoryViewModel cvm = new CategoryViewModel(menuManager);
            ActivateItemAsync(cvm, System.Threading.CancellationToken.None);
        }

        public void ShowPreview()
        {
            PreviewViewModel vm = new PreviewViewModel();
            ActivateItemAsync(vm, System.Threading.CancellationToken.None);
        }

        public void TestSQL()
        {
            DataAccess db = new DataAccess();
            MessageBox.Show(db.GetJotain("Soup"));
        }

        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
