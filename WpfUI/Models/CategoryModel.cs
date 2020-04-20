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

        public string Name 
        { 
            get { return TheDish.Name; } 
            set { TheDish.Name = value; }
        }

        public string Description 
        { 
            get { return TheDish.Description; }
            set { TheDish.Description = value; }
        }

        public double Price 
        { 
            get { return TheDish.Price; }
            set { TheDish.Price = value; }
        }

        public bool ContainsLactose 
        { 
            get { return TheDish.ContainsLactose; }
            set { TheDish.ContainsLactose = value; }
        }

        public bool ContainsGluten 
        { 
            get { return TheDish.ContainsGluten; }
            set { TheDish.ContainsGluten = value; }
        }

        public bool ContainsFish 
        { 
            get { return TheDish.ContainsFish; }
            set { TheDish.ContainsFish = value; }
        }

        public Menu.Category CategoryOfDish { get; set; } = Menu.Category.Starter;

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
                    CategoryOfDish = (Menu.Category)cat;
                }                
            }
        }

        public CategoryModel(Dish dish)
        {
            this.TheDish = dish;
        }



    }
}
