using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUI.MenuLibrary;
using WpfUI.Models;

namespace WpfUI.ViewModels
{
    public class CategoryViewModel : Screen
    {
        public MenuManager TheMenuManager { get; set; }

        public BindableCollection<Dish> DishesInCategory { get; } = new BindableCollection<Dish>();

        public List<MenuCategory> CategoryNames { get; set; }

        public List<Menu> Menus { get; }

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

        private Menu _selectedMenu;
        public Menu SelectedMenu 
        {
            get { return _selectedMenu; }
            set
            {
                _selectedMenu = value;
                CategoryNames = SelectedMenu.Categories;
                NotifyOfPropertyChange(() => CategoryNames);
                SelectedCategory = SelectedMenu.Categories[0]; // this calls LoadCategory()
                NotifyOfPropertyChange(() => SelectedCategory);
            }
        }

        public List<Dish> AllDishes { get; set; }

        public Dish SelectedDishInCategory { get; set; }

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
            Menus = manager.AllMenus;
            SelectedCategory = SelectedMenu.Categories[0];
            AllDishes = manager.AllDishes;
        }

        public void AddDishToCategory()
        {
            //System.Windows.MessageBox.Show(
            //    $"Move {(SelectedDishCategory == null ? "null" : SelectedDishCategory.Name)}: {currentCategory} -> {SelectedCategoryForMove.Name}" +
            //    ", " + DishesInCategory.Contains(SelectedDishCategory));
            
            if (SelectedCategory == null)
            {
                System.Windows.MessageBox.Show("No category has been selected");
                return;
            }
            if (SelectedDishInAllDishes == null)
            {
                System.Windows.MessageBox.Show("No dish has been selected");
                return;
            }

            Dish dish = SelectedDishInAllDishes;
            if (SelectedCategory.Dishes.Contains(dish))
            {
                System.Windows.MessageBox.Show($"The dish {dish.Name} is already in category {SelectedCategory.Id}");
                return;
            }

            DishesInCategory.Add(dish);
            SelectedCategory.Dishes.Add(dish);
            SelectedDishInCategory = dish;
            NotifyOfPropertyChange(() => DishesInCategory);
        }
    }
}
