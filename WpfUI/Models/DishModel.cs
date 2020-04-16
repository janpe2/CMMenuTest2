using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using WpfUI.MenuLibrary;

namespace WpfUI.Models
{
    public class DishModel
    {
        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        private static List<string> _categoryNames = new List<string>(MenuCategory.CategoryNames);
        public List<string> CategoryNames { get; } = _categoryNames;

        public DishModel(Dish d)
        {
            Name = d.Name;
            Price = d.Price;
            Description = d.Description;
        }
    }
}
