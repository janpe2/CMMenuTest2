using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WpfUI.MenuLibrary;
using WpfUI.Models;

namespace WpfUI.ViewModels
{
    public class CategoryViewModel : Screen
    {
        public MenuManager TheMenuManager { get; set; }

        public BindableCollection<Dish> DishesInCategory { get; } = new BindableCollection<Dish>();

        public List<MenuCategory> CategoryNames { get; set; }

        public BindableCollection<Menu> Menus { get; } = new BindableCollection<Menu>();

        private MenuCategory _selectedCategory;
        public MenuCategory SelectedCategory 
        { 
            get { return _selectedCategory;  } 
            set
            {
                if (value != null)
                {
                    _selectedCategory = value;
                    LoadCategory();
                }
            }
        }

        public string SelectedMenuDescription 
        {
            get 
            { 
                return SelectedMenu.Description; 
            }
            set
            {
                SelectedMenu.Description = value;
                NotifyOfPropertyChange(() => SelectedMenuDescription);
            }
        }

        private Menu _selectedMenu;
        public Menu SelectedMenu 
        {
            get 
            { 
                return _selectedMenu;
            }
            set
            {
                if (value != null)
                {
                    _selectedMenu = value;
                    CategoryNames = value.Categories;
                    SelectedMenuDescription = value.Description;
                    SelectedCategory = value.Categories[0]; // this calls LoadCategory()
                    NotifyOfPropertyChange(() => CategoryNames);
                    NotifyOfPropertyChange(() => SelectedCategory);
                    NotifyOfPropertyChange(() => SelectedMenu);
                }
            }
        }

        public List<Dish> AllDishes { get; set; }

        private Dish _selectedDishInCategory;
        public Dish SelectedDishInCategory 
        { 
            get { return _selectedDishInCategory; }
            set
            {
                _selectedDishInCategory = value;
                NotifyOfPropertyChange(() => SelectedDishInCategory);
            }
        }

        public Dish SelectedDishInAllDishes { get; set; }

        private void LoadCategory()
        {
            DishesInCategory.Clear();
            DishesInCategory.AddRange(SelectedCategory.Dishes);

            if (DishesInCategory.Count == 0)
            {
                SelectedDishInCategory = null;
            }
            else
            {
                SelectedDishInCategory = DishesInCategory[0];
            }

            NotifyOfPropertyChange(() => DishesInCategory);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="manager">menu manager</param>
        public CategoryViewModel(MenuManager manager)
        {
            TheMenuManager = manager;
            SelectedMenu = manager.AllMenus[0];
            CategoryNames = SelectedMenu.Categories;
            Menus.AddRange(manager.AllMenus);
            SelectedCategory = SelectedMenu.Categories[0];
            AllDishes = manager.GetAllDishesSortedAlphabetically();
        }

        public void AddDishToCategory()
        {
            if (SelectedCategory == null)
            {
                MessageBox.Show("No category has been selected");
                return;
            }
            if (SelectedDishInAllDishes == null)
            {
                MessageBox.Show("No dish has been selected");
                return;
            }

            Dish dish = SelectedDishInAllDishes;
            if (SelectedCategory.Dishes.Contains(dish))
            {
                MessageBox.Show($"The dish {dish.Name} is already in category {SelectedCategory.Id}");
                return;
            }

            DishesInCategory.Add(dish);
            SelectedCategory.Dishes.Add(dish);
            SelectedDishInCategory = dish;
            NotifyOfPropertyChange(() => DishesInCategory);
        }

        public void RemoveDishFromCategory()
        {
            if (SelectedDishInCategory == null)
            {
                MessageBox.Show("No dish has been selected in category");
                return;
            }

            Dish dish = SelectedDishInCategory;
            DishesInCategory.Remove(dish);
            SelectedCategory.Dishes.Remove(dish);
            SelectedDishInCategory = null;
            NotifyOfPropertyChange(() => DishesInCategory);
        }
        public void AddMenu()
        {
            string name = AskMenuName("New Menu", "Name for new menu:", "");
            if (name == null)
            {
                return;
            }

            Menu menu = new Menu(name, "");
            TheMenuManager.AllMenus.Add(menu);
            Menus.Add(menu);
            NotifyOfPropertyChange(() => Menus);
            SelectedMenu = menu;
        }

        public void DeleteMenu()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(
                $"Are you sure you want delete menu {SelectedMenu.Name}?", "Delete Menu", 
                MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                int index = TheMenuManager.AllMenus.IndexOf(SelectedMenu);
                Menu newSelection;

                if (index == -1)
                {
                    return;
                }
                else if (TheMenuManager.AllMenus.Count == 1)
                {
                    // TODO
                    return; // newSelection = null;
                }
                else if (index == 0)
                {
                    newSelection = TheMenuManager.AllMenus[1];
                }
                else
                {
                    newSelection = TheMenuManager.AllMenus[index - 1];
                }

                TheMenuManager.AllMenus.RemoveAt(index);
                SelectedMenu = newSelection;

                Menus.Clear();
                Menus.AddRange(TheMenuManager.AllMenus);
                NotifyOfPropertyChange(() => Menus);
            }
        }

        private string AskMenuName(string title, string message, string oldName)
        {
            var viewModel = IoC.Get<InputBoxViewModel>();
            viewModel.DialogTitle = title;
            viewModel.MessageText = message;
            viewModel.InputText = oldName;
            new WindowManager().ShowDialogAsync(viewModel);

            if (!viewModel.DialogResult || string.IsNullOrWhiteSpace(viewModel.InputText) ||
                viewModel.InputText == oldName)
            {
                return null;
            }
            return viewModel.InputText;
        }

        public void RenameMenu()
        {
            if (SelectedMenu == null)
            {
                return;
            }

            string newName = AskMenuName("Rename Menu", "Rename menu to:", SelectedMenu.Name);
            if (newName == null)
            {
                return;
            }

            SelectedMenu.Name = newName;
            NotifyOfPropertyChange(() => SelectedMenu);
        }


    }
}
