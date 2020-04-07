using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WpfUI.MenuLibrary;
using WpfUI.Models;

namespace WpfUI.ViewModels
{
    public class DishViewModel : Screen
    {
        public MenuManager TheMenuManager { get; set; }

        private BindableCollection<Dish> _dishes = new BindableCollection<Dish>();
        public BindableCollection<Dish> Dishes 
        { 
            get
            {
                return _dishes;
            }
        }

        private Dish _selectedDish;
        public Dish SelectedDish 
        { 
            get
            {
                return _selectedDish;
            }
            set
            {
                _selectedDish = value;
                NameOfSelectedDish = _selectedDish.Name;
                PriceOfSelectedDish = $"{_selectedDish.Price}";
                DescriptionOfSelectedDish = _selectedDish.Description;
            }
        }

        private string _nameOfSelectedDish;
        public string NameOfSelectedDish
        {
            get { return _nameOfSelectedDish; }
            set
            {
                _nameOfSelectedDish = value;
                NotifyOfPropertyChange(() => NameOfSelectedDish);
            }
        }

        private string _priceOfSelectedDish;
        public string PriceOfSelectedDish
        {
            get { return _priceOfSelectedDish;  } 
            set
            {
                _priceOfSelectedDish = value;
                NotifyOfPropertyChange(() => PriceOfSelectedDish);
            }
        }

        private string _descriptionOfSelectedDish;
        public string DescriptionOfSelectedDish
        {
            get { return _descriptionOfSelectedDish;  }
            set
            {
                _descriptionOfSelectedDish = value;
                NotifyOfPropertyChange(() => DescriptionOfSelectedDish);
            }
        }

        public DishViewModel(MenuManager m)
        {
            TheMenuManager = m;
            _dishes.AddRange(m.AllDishes);
            if (m.AllDishes.Count > 0)
            {
                SelectedDish = m.AllDishes[0];
            }
        }

        private bool ParsePrice(out double price)
        {
            bool ok = double.TryParse(PriceOfSelectedDish, out price);
            if (!ok)
            {
                MessageBox.Show("Price is not a valid number", "Invalid Number");
            }
            return ok;
        }

        public void AddDish()
        {
            double price;
            bool ok = ParsePrice(out price);
            if (!ok)
            {
                return;
            }
            string name = NameOfSelectedDish;
            string descr = DescriptionOfSelectedDish;
            Dish dish = new Dish(name, descr, price);
            Dishes.Add(dish);
            TheMenuManager.AllDishes.Add(dish);
            SelectedDish = dish;
            NotifyOfPropertyChange(() => SelectedDish);
            MessageBox.Show($"New dish named '{name}' was added", "New Dish");
        }

        public void SaveChanges()
        {
            double price;
            bool ok = ParsePrice(out price);
            if (!ok)
            {
                return;
            }
            int index = TheMenuManager.AllDishes.IndexOf(SelectedDish);
            Dish newDish = new Dish(NameOfSelectedDish, DescriptionOfSelectedDish, price);

            SelectedDish = newDish;
            TheMenuManager.AllDishes[index] = newDish;
            Dishes[index] = newDish;
            NotifyOfPropertyChange(() => SelectedDish);

            MessageBox.Show("Changes were saved to dish", "Save Dish");

            // TODO If name is changed, the new name appears in the combo items but not in the selected item.
        }

        public void DeleteDish()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(
                $"Are you sure you want to delete dish '{NameOfSelectedDish}'?", 
                "Delete Confirmation", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                int index = TheMenuManager.AllDishes.IndexOf(SelectedDish);
                if (TheMenuManager.AllDishes.Count == 1 || index < 0)
                {
                    // TODO List becomes empty. Program crashes.
                    MessageBox.Show("Error");
                    return;
                }

                Dish newSelection = null;
                if (index == 0)
                {
                    newSelection = TheMenuManager.AllDishes[1];
                }
                else
                {
                    newSelection = TheMenuManager.AllDishes[index - 1];
                }

                TheMenuManager.AllDishes.RemoveAt(index);
                Dishes.RemoveAt(index);
                SelectedDish = newSelection;
                NotifyOfPropertyChange(() => SelectedDish);
            }
        }
    }
}
