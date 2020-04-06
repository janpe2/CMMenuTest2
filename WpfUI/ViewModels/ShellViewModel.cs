using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WpfUI.Models;

namespace WpfUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public BindableCollection<DishModel> DishesList { get; } = new BindableCollection<DishModel>
        {
            new DishModel("Kalakeitto", "Perunaa, haukea ja kermaa", 10.95),
            new DishModel("Pippuripihvi", "Naudanpihvi ja pippuria", 12.85),
            new DishModel("Hernekeitto", "Herneitä ja vettä", 9.55)
        };

        private DishModel _selectedDish;
        public DishModel SelectedDish 
        { 
            get 
            { 
                return _selectedDish; 
            }
            set 
            {
                _selectedDish = value;
                LoadDish();
            }
        }

        public ShellViewModel()
        {
            SelectedDish = DishesList[0];
        }

        public void LoadDish()
        {
            //MessageBox.Show("Loading dish");
            DishViewModel dvm = new DishViewModel(SelectedDish);
            ActivateItemAsync(dvm, System.Threading.CancellationToken.None);
        }
    }
}
