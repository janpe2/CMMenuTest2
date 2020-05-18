using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WpfUI.MenuLibrary;
using WpfUI.MenuLibrary.DataAccess;


namespace WpfUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public ShellViewModel()
        {
            ShowDishes();
        }

        public void ShowDishes()
        {
            try
            {
                DishViewModel dvm = new DishViewModel();
                ActivateItemAsync(dvm, System.Threading.CancellationToken.None);
            }
            catch (Exception)
            {
            }
        }

        public void ShowMenus()
        {
            try
            {
                CategoryViewModel cvm = new CategoryViewModel();
                ActivateItemAsync(cvm, System.Threading.CancellationToken.None);
            }
            catch (Exception)
            {
            }
        }

        public void ShowPreview()
        {
            try
            {
                PreviewViewModel vm = new PreviewViewModel();
                ActivateItemAsync(vm, System.Threading.CancellationToken.None);
            }
            catch (Exception)
            {
            }
        }

        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
