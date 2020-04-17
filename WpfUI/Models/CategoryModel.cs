using System;
using System.Collections.Generic;
using System.Text;
using WpfUI.MenuLibrary;

namespace WpfUI.Models
{
    public class CategoryModel
    {
        public List<string> CategoryNames { get; set; } = new List<string>(MenuCategory.CategoryNames);

        public Dish TheDish { get; set; }

        public string Name { get { return TheDish.Name; } }

        public string Description { get { return TheDish.Description; } }

        public double Price { get { return TheDish.Price; } }

        public bool ContainsLactose { get { return TheDish.ContainsLactose; } }

        public bool ContainsGluten { get { return TheDish.ContainsGluten; } }

        public bool ContainsFish { get { return TheDish.ContainsFish; } }

        public MenuCategory.Category CategoryOfDish { get; set; } = MenuCategory.Category.Starter;

        public string CategoryOfDishAsString 
        {
            get
            {
                return CategoryOfDish.ToString();
            }
            set 
            {
                int cat;
                if (int.TryParse(value, out cat))
                {
                    CategoryOfDish = (MenuCategory.Category)cat;
                }                
            }
        }

        public CategoryModel(Dish dish)
        {
            this.TheDish = dish;
        }



    }
}
