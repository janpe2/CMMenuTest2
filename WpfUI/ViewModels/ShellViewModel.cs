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
            
            Dish dish = db.GetDish("Fish soup");
            if (dish == null)
            {
                MessageBox.Show("No dish");
            }
            else
            {
                MessageBox.Show($"{dish.Name} {dish.Description} {dish.Price}");
            }
            

            /*
            db.InsertDish("New dish", "Something new stuff", 8.45);

            List<Dish> dishes = db.GetAllDishes();
            StringBuilder sb = new StringBuilder();
            foreach (var item in dishes)
            {
                sb.Append(item.Name.Trim()).Append(" -- ").Append(item.Description.Trim()).Append('\n');
            }
            MessageBox.Show(sb.ToString());
            */
        }

        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
