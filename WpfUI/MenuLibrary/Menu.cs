using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.MenuLibrary
{
    public class Menu
    {
        /// <summary>
        /// Category for dishes in a menu.
        /// </summary>
        public enum Category
        {
            Starter = 0,
            MainCourse,
            Dessert,
            Drink,
            Uncategorized
        }

        public string Description { get; }

        public List<MenuCategory> Categories { get; } = new List<MenuCategory>();
        public Menu(string descr)
        {
            Description = descr;
        }

        public void AddDish(Category category, string name, string descr, double price)
        {
            Dish dish = new Dish(name, descr, price);
            Categories[(int)category].Dishes.Add(dish);
        }


        
    }
}
