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

        public BindableCollection<CategoryModel> DishesInCategory { get; } = new BindableCollection<CategoryModel>();

        //public string[] CategoryNames { get; } = MenuCategory.CategoryNames;

        public List<MenuCategory> AllCategories { get; }

        public CategoryModel SelectedDishCategory { get; set; }

        public MenuCategory SelectedCategoryForMove { get; set; }

        // TODO remove
        public string CategoryOfSelectedDish { get; } = "Main";

        private MenuCategory.Category currentCategory = MenuCategory.Category.Starter;

        public bool CurrentCategoryMain 
        { 
            get { return currentCategory == MenuCategory.Category.Main; }
            set 
            { 
                currentCategory = MenuCategory.Category.Main;
                updateDishes();
            }
        }

        public bool CurrentCategoryStarter
        {
            get { return currentCategory == MenuCategory.Category.Starter; }
            set 
            { 
                currentCategory = MenuCategory.Category.Starter;
                updateDishes();
            }
        }

        public bool CurrentCategoryDessert
        {
            get { return currentCategory == MenuCategory.Category.Dessert; }
            set 
            { 
                currentCategory = MenuCategory.Category.Dessert;
                updateDishes();
            }
        }

        public bool CurrentCategoryDrink
        {
            get { return currentCategory == MenuCategory.Category.Drink; }
            set 
            { 
                currentCategory = MenuCategory.Category.Drink;
                updateDishes();
            }
        }

        public bool CurrentCategoryUncategorized
        {
            get { return currentCategory == MenuCategory.Category.Uncategorized; }
            set 
            { 
                currentCategory = MenuCategory.Category.Uncategorized;
                updateDishes();
            }
        }

        private void updateDishes()
        {
            MenuCategory cat = TheMenuManager.AllCategories[(int)currentCategory];
            DishesInCategory.Clear();
            foreach (var dish in cat.Dishes)
            {
                DishesInCategory.Add(new CategoryModel(dish));
            }

            if (cat.Dishes.Count == 0)
            {
                SelectedDishCategory = null;
            }
            else
            {
                SelectedDishCategory = DishesInCategory[0];
            }

            NotifyOfPropertyChange(() => SelectedDishCategory);
            NotifyOfPropertyChange(() => DishesInCategory);
            NotifyOfPropertyChange(() => CurrentCategoryStarter);
            NotifyOfPropertyChange(() => CurrentCategoryMain);
            NotifyOfPropertyChange(() => CurrentCategoryDessert);
            NotifyOfPropertyChange(() => CurrentCategoryDrink);
            NotifyOfPropertyChange(() => CurrentCategoryUncategorized);
        }

        public CategoryViewModel(MenuManager manager)
        {
            TheMenuManager = manager;
            AllCategories = manager.AllCategories;
            SelectedCategoryForMove = manager.AllCategories[0];
            MenuCategory cat = TheMenuManager.AllCategories[(int)MenuCategory.Category.Starter];
            foreach (var dish in cat.Dishes)
            {
                DishesInCategory.Add(new CategoryModel(dish));
            }
        }

        public void MoveDishToCategory()
        {
            //System.Windows.MessageBox.Show(
            //    $"Move {(SelectedDishCategory == null ? "null" : SelectedDishCategory.Name)}: {currentCategory} -> {SelectedCategoryForMove.Name}" +
            //    ", " + DishesInCategory.Contains(SelectedDishCategory));

            if (SelectedDishCategory == null || !DishesInCategory.Contains(SelectedDishCategory))
            {
                System.Windows.MessageBox.Show("No dish has been selected");
                return;
            }

            MenuCategory sourceCategory = TheMenuManager.AllCategories[(int)currentCategory];
            Dish dish = SelectedDishCategory.TheDish;
            if (SelectedCategoryForMove.Dishes.Contains(dish))
            {
                System.Windows.MessageBox.Show($"The dish {dish.Name} is already in category {SelectedCategoryForMove.Id}");
                return;
            }

            sourceCategory.Dishes.Remove(dish);
            DishesInCategory.Remove(SelectedDishCategory);
            SelectedCategoryForMove.Dishes.Add(dish);

            NotifyOfPropertyChange(() => DishesInCategory);
        }
    }
}
