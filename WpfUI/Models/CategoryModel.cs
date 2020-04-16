using System;
using System.Collections.Generic;
using System.Text;
using WpfUI.MenuLibrary;

namespace WpfUI.Models
{
    public class CategoryModel
    {
        public List<string> CategoryNames { get; } = _categoryNames;

        private static List<string> _categoryNames = new List<string>(MenuCategory.CategoryNames);

        private Dish dish;

        public string Name { get { return dish.Name; } }

        public string Description { get { return dish.Description; } }

        public double Price { get { return dish.Price; } }

        public bool ContainsLactose { get { return dish.ContainsLactose; } }

        public bool ContainsGluten { get { return dish.ContainsGluten; } }

        public bool ContainsFish { get { return dish.ContainsFish; } }

        public CategoryModel(Dish dish)
        {
            this.dish = dish;
        }



    }
}
