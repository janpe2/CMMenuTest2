using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.Models
{
    public class DishModel
    {
        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public DishModel(string name, string descr, double price)
        {
            Name = name;
            Price = price;
            Description = descr;
        }
    }
}
