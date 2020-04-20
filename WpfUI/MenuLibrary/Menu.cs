﻿using System;
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

        public string Name { get; }

        public string Description { get; }

        public List<MenuCategory> Categories { get; } = new List<MenuCategory>();

        public Menu(string name, string descr)
        {
            Name = name;
            Description = descr;

            Categories.Add(new MenuCategory(Menu.Category.Starter));
            Categories.Add(new MenuCategory(Menu.Category.MainCourse));
            Categories.Add(new MenuCategory(Menu.Category.Dessert));
            Categories.Add(new MenuCategory(Menu.Category.Drink));
            Categories.Add(new MenuCategory(Menu.Category.Uncategorized));
        }

        public void AddDish(Category category, string name, string descr, double price)
        {
            Dish dish = new Dish(name, descr, price);
            Categories[(int)category].Dishes.Add(dish);
        }


        
    }
}
