﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.MenuLibrary
{
    public class MenuManager
    {
        public List<Dish> AllDishes { get; set; } = new List<Dish>();

        public List<MenuCategory> AllCategories { get; set; } = new List<MenuCategory>();

        public List<Menu> AllMenus { get; set; } = new List<Menu>();

        public void LoadSampleData()
        {
            Menu menu1 = new Menu("Lounasmenu");
            AllMenus.Add(menu1);
            AddDish(Menu.Category.Starter, "Sipulikeitto", "Sipulia ja lientä", 6.55, menu1);
            AddDish(Menu.Category.MainCourse, "Pippuripihvi", "Pippurilla maustettu naudan pihvi", 16.95, menu1);
            AddDish(Menu.Category.MainCourse, "Lehtipihvi", "Ohut pihvi ja kastiketta", 10.85, menu1);
            AddDish(Menu.Category.MainCourse, "Kanafilee", "Mureaa kanan fileetä ja riisiä", 11.05, menu1);
            AddDish(Menu.Category.Dessert, "Suklaakakku", "Suklaista kakkua ja kermavaahtoa", 7.45, menu1);
            //menu1.AddDrink("Vesi", "Puhdasta vettä", 1.50, 0.0);
            //menu1.AddDrink("Olut", "Hienoa olutta", 4.95, 4.7);

            Menu menu2 = new Menu("A la carte -menu");
            AllMenus.Add(menu2);
            AddDish(Menu.Category.MainCourse, "Pekonipihvi", "Naudan pihvi ja pekonia", 17.95, menu2);
        }

        public void AddDish(Menu.Category category, string name, string descr, double price, Menu menu)
        {
            Dish dish = new Dish(name, descr, price);
            //menu.AddDish();
            AllDishes.Add(dish);
        }

    }
}
