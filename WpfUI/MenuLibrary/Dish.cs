using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.MenuLibrary
{
    public class Dish
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public bool ContainsLactose { get; set; }

        public bool ContainsGluten { get; set; }

        public bool ContainsFish { get; set; }

        // TODO ?
        public List<string> CategoryNames { get; } = new List<string>(MenuCategory.CategoryNames);

        public Dish(string name, string descr, double price)
        {
            Name = name;
            Price = price;
            Description = descr;
        }

        public Dish(string name, string descr, double price, bool lactose, bool gluten, bool fish)
        {
            Name = name;
            Price = price;
            Description = descr;
            ContainsLactose = lactose;
            ContainsGluten = gluten;
            ContainsFish = fish;
        }
    }
}
