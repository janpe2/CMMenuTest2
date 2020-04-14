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
                NotifyOfPropertyChange(() => SelectedDish);
                NameOfSelectedDish = _selectedDish.Name;
                PriceOfSelectedDish = $"{_selectedDish.Price}";
                DescriptionOfSelectedDish = _selectedDish.Description;

                SelectedDishContainsLactose = _selectedDish.ContainsLactose;
                SelectedDishContainsGluten = _selectedDish.ContainsGluten;
                SelectedDishContainsFish = _selectedDish.ContainsFish;

                SelectedDishModified = false;
            }
        }

        private string _nameOfSelectedDish;
        public string NameOfSelectedDish
        {
            get 
            { 
                return _nameOfSelectedDish; 
            }
            set
            {
                _nameOfSelectedDish = value;
                SelectedDishModified = true;
                NotifyOfPropertyChange(() => NameOfSelectedDish);
            }
        }

        private string _priceOfSelectedDish;
        public string PriceOfSelectedDish
        {
            get 
            { 
                return _priceOfSelectedDish;  
            } 
            set
            {
                _priceOfSelectedDish = value;
                SelectedDishModified = true;
                NotifyOfPropertyChange(() => PriceOfSelectedDish);
            }
        }

        private string _descriptionOfSelectedDish;
        public string DescriptionOfSelectedDish
        {
            get 
            { 
                return _descriptionOfSelectedDish;  
            }
            set
            {
                _descriptionOfSelectedDish = value;
                SelectedDishModified = true;
                NotifyOfPropertyChange(() => DescriptionOfSelectedDish);
            }
        }

        private bool _selectedDishContainsLactose;
        public bool SelectedDishContainsLactose
        {
            get 
            { 
                return _selectedDishContainsLactose; 
            }
            set 
            { 
                _selectedDishContainsLactose = value;
                SelectedDishModified = true;
                NotifyOfPropertyChange(() => SelectedDishContainsLactose);
            }
        }

        private bool _selectedDishContainsGluten;
        public bool SelectedDishContainsGluten
        {
            get 
            { 
                return _selectedDishContainsGluten; 
            }
            set 
            { 
                _selectedDishContainsGluten = value;
                SelectedDishModified = true;
                NotifyOfPropertyChange(() => SelectedDishContainsGluten);
            }
        }

        private bool _selectedDishContainsFish;
        public bool SelectedDishContainsFish
        {
            get 
            { 
                return _selectedDishContainsFish; 
            }
            set 
            { 
                _selectedDishContainsFish = value;
                SelectedDishModified = true;
                NotifyOfPropertyChange(() => SelectedDishContainsFish);
            }
        }

        private bool _selectedDishModified;
        public bool SelectedDishModified
        {
            get 
            { 
                return _selectedDishModified; 
            }
            set
            {
                _selectedDishModified = value;
                NotifyOfPropertyChange(() => SelectedDishModified);
                NotifyOfPropertyChange(() => IsSelectedDishSaved);

                SelectedDishModifiedText = value ? "Modified" : "Saved";
                SelectedDishModifiedColor = value ? "Red" : "Green";
            }
        }

        private string _selectedDishModifiedText = "Saved";
        public string SelectedDishModifiedText 
        { 
            get
            {
                return _selectedDishModifiedText;
            }
            set
            {
                _selectedDishModifiedText = value;
                NotifyOfPropertyChange(() => SelectedDishModifiedText);
            }
        }

        private string _selectedDishModifiedColor = "Green";
        public string SelectedDishModifiedColor 
        {
            get
            {
                return _selectedDishModifiedColor;
            }
            set
            {
                _selectedDishModifiedColor = value;
                NotifyOfPropertyChange(() => SelectedDishModifiedColor);
            }
        }

        /// <summary>
        /// Read-only inversion of SelectedDishModified.
        /// </summary>
        public bool IsSelectedDishSaved 
        { 
            get { return !SelectedDishModified; }
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
            if (!AskToSaveChanges())
            {
                return;
            }

            Dish dish = new Dish("New Dish", "", 0.00);
            Dishes.Add(dish);
            TheMenuManager.AllDishes.Add(dish);
            SelectedDish = dish;
            SelectedDishModified = false;
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
            newDish.ContainsLactose = SelectedDishContainsLactose;
            newDish.ContainsGluten = SelectedDishContainsGluten;
            newDish.ContainsFish = SelectedDishContainsFish;

            TheMenuManager.AllDishes[index] = newDish;
            Dishes[index] = newDish;
            SelectedDish = newDish;
            SelectedDishModified = false;
            //MessageBox.Show($"Changes were saved to '{newDish.Name}'", "Save Dish");
        }

        public void DiscardChanges()
        {
            if (!SelectedDishModified)
            {
                return;
            }
            MessageBoxResult messageBoxResult = MessageBox.Show(
                $"Do you really want to discard all changes you made to dish '{NameOfSelectedDish}'?",
                "Discard Changes?", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SelectedDish = _selectedDish; // to notify all
            }
        }

        public void DeleteDish()
        {
            if (TheMenuManager.AllDishes.Count <= 1)
            {
                // TODO List becomes empty. Program crashes.
                //MessageBox.Show("You can't delete the last dish.");
                //return;
            }

            MessageBoxResult messageBoxResult = MessageBox.Show(
                $"Are you sure you want to delete dish '{NameOfSelectedDish}'?", 
                "Delete Confirmation", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                int index = TheMenuManager.AllDishes.IndexOf(SelectedDish);
                if (index < 0)
                {
                    MessageBox.Show("Error");
                    return;
                }
                
                Dish newSelection;
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
                SelectedDishModified = false;
            }
        }

        /// <summary>
        /// Asks the user and saves changes if the user chooses so.
        /// </summary>
        /// <returns>false if operation should be stopped; true if it can continue</returns>
        private bool AskToSaveChanges()
        {
            if (!SelectedDishModified)
            {
                return true;
            }
            MessageBoxResult messageBoxResult = MessageBox.Show(
                $"Do you want to save changes to dish '{NameOfSelectedDish}'?",
                "Save?", MessageBoxButton.YesNoCancel);

            switch (messageBoxResult)
            {
                case MessageBoxResult.Yes:
                    SaveChanges();
                    return true;
                case MessageBoxResult.No:
                    return true;
                default:
                    return false;
            }
        }
    }
}
