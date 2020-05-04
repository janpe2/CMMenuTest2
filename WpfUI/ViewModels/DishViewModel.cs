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
    public class DishViewModel : Screen
    {
        //public MenuManager TheMenuManager { get; set; }

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

                string name, descr, price;
                bool lactose, gluten, fish;
                int id;

                if (_selectedDish == null)
                {
                    name = "";
                    price = "";
                    descr = "";
                    lactose = false;
                    gluten = false;
                    fish = false;
                    id = -1;
                }
                else
                {
                    name = _selectedDish.Name;
                    price = $"{_selectedDish.Price}";
                    descr = _selectedDish.Description;
                    lactose = _selectedDish.ContainsLactose;
                    gluten = _selectedDish.ContainsGluten;
                    fish = _selectedDish.ContainsFish;
                    id = _selectedDish.Id;
                }

                NotifyOfPropertyChange(() => SelectedDish);
                NameOfSelectedDish = name;
                PriceOfSelectedDish = price;
                IdOfSelectedDish = id;
                DescriptionOfSelectedDish = descr;
                SelectedDishContainsLactose = lactose;
                SelectedDishContainsGluten = gluten;
                SelectedDishContainsFish = fish;
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

        private int _idOfSelectedDish;
        public int IdOfSelectedDish 
        { 
            get { return _idOfSelectedDish; }
            set
            {
                _idOfSelectedDish = value;
                NotifyOfPropertyChange(() => IdOfSelectedDish);
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
            //TheMenuManager = m;

            DataAccess da = new DataAccess();
            List<Dish> allDishes = da.GetAllDishes();
            _dishes.AddRange(TrimNames(allDishes));

            if (m.AllDishes.Count > 0)
            {
                SelectedDish = _dishes[0];
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

        private bool IsDishNameInUse(string name, Dish currentDish)
        {
            foreach (var d in Dishes)
            {
                if (d.Name == name && d != currentDish)
                {
                    return true;
                }
            }
            return false;
        }

        private List<Dish> TrimNames(List<Dish> list)
        {
            foreach (var d in list)
            {
                d.Name = d.Name.Trim();
                d.Description = d.Description.Trim();
            }
            return list;
        }

        public void AddDish()
        {
            if (!AskToSaveChanges())
            {
                return;
            }

            // Create a unique name by appending an integer
            string name;
            int number = Dishes.Count + 1;
            while (true)
            {
                name = $"New dish {number}";
                if (IsDishNameInUse(name, null))
                {
                    number++;
                }
                else
                {
                    break;
                }
            }

            string descr = "";
            double price = 0.00;
            Dish dish = new Dish(name, descr, price);

            DataAccess da = new DataAccess();
            int id = da.InsertDish(dish);
            dish.Id = id; // GetMaxIdFromDB(da); // get the Id of the Dish we just added to the database
            Dishes.Add(dish);
            //TheMenuManager.AllDishes.Add(dish);



            SelectedDish = dish;
            SelectedDishModified = false;
        }

        public void SaveChanges()
        {
            if (string.IsNullOrEmpty(NameOfSelectedDish))
            {
                MessageBox.Show("Name must not be empty", "Save Dish");
                return;
            }

            double price;
            bool ok = ParsePrice(out price);
            if (!ok)
            {
                return;
            }

            string oldName = _selectedDish.Name, newName = NameOfSelectedDish;

            /*// TODO This does not find duplicate names!
            if (IsDishNameInUse(newName, _selectedDish))
            {
                MessageBox.Show($"Dish named {newName} already exists. Please write a different name.", "Save Dish");
                return;
            }
            */

            int index = Dishes.IndexOf(SelectedDish);
            bool isRenamed = oldName != newName;

            _selectedDish.Name = NameOfSelectedDish;
            _selectedDish.Description = DescriptionOfSelectedDish;
            _selectedDish.Price = price;
            _selectedDish.ContainsLactose = SelectedDishContainsLactose;
            _selectedDish.ContainsGluten = SelectedDishContainsGluten;
            _selectedDish.ContainsFish = SelectedDishContainsFish;

            DataAccess da = new DataAccess();
            da.ModifyDish(_selectedDish);

            if (isRenamed)
            {
                // Reload the list to force items update in ComboBox
                Dishes.Clear();
                Dishes.AddRange(TrimNames(new DataAccess().GetAllDishes()));

                NotifyOfPropertyChange(() => Dishes);
                NotifyOfPropertyChange(() => SelectedDish);
            }
            SelectedDishModified = false;
        }

        public void DiscardChanges()
        {
            if (!SelectedDishModified || SelectedDish == null)
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
            if (Dishes.Count == 0 || SelectedDish == null)
            {
                return;
            }

            MessageBoxResult messageBoxResult = MessageBox.Show(
                $"Are you sure you want to delete dish '{NameOfSelectedDish}'?", 
                "Delete Confirmation", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                int index = Dishes.IndexOf(SelectedDish);
                if (index < 0)
                {
                    MessageBox.Show("Dish does not exist in list");
                    return;
                }
                
                Dish newSelection;
                if (Dishes.Count == 1)
                {
                    newSelection = null;
                }
                else if (index == 0)
                {
                    newSelection = Dishes[1];
                }
                else
                {
                    newSelection = Dishes[index - 1];
                }

                DataAccess da = new DataAccess();
                da.DeleteDish(SelectedDish);

                //TheMenuManager.AllDishes.RemoveAt(index);
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

        private int GetMaxId()
        {
            int id = 1;
            foreach (var d in Dishes)
            {
                if (d.Id > id)
                {
                    id = d.Id;
                }
            }
            return id;
        }

        private int GetMaxIdFromDB(DataAccess da)
        {
            List<Dish> dishes = da.GetAllDishes();
            int id = 1;
            foreach (var d in dishes)
            {
                if (d.Id > id)
                {
                    id = d.Id;
                }
            }
            return id;
        }
    }
}
