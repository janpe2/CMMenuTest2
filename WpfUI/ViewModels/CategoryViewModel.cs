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

        public BindableCollection<CategoryModel> DishesList { get; } = new BindableCollection<CategoryModel>();

        // TODO
        public BindableCollection<string> NamesOfCategories { get; } = new BindableCollection<string>()
        {
            "Starterr", "Main", "Dessert", "Drink", "Other"
        };

        // TODO
        public string CategoryOfSelectedDish { get; } = "Main";

        public CategoryViewModel(MenuManager manager)
        {
            TheMenuManager = manager;
            foreach (var dish in manager.AllDishes)
            {
                DishesList.Add(new CategoryModel(dish));
            }
            //DishesList.AddRange(manager.AllDishes);
            
            //System.Windows.MessageBox.Show(string.Join(',', MenuCategory.CategoryNames));
        }
    }
}
