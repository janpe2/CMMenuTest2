using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.MenuLibrary
{
    public class Dish
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public Dish(string name, string descr, double price)
        {
            Name = name;
            Price = price;
            Description = descr;
        }
    }
}
