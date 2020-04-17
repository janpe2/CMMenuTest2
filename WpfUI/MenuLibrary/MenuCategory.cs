using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.MenuLibrary
{
    public class MenuCategory
    {
		public enum Category
		{
			Starter,
			Main,
			Dessert,
			Drink,
			Uncategorized
		}

		public static string[] CategoryNames = Enum.GetNames(typeof(Category));		

		/// <summary>
		/// Name of the category.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Dishes that belong to the category.
		/// </summary>
		public List<Dish> Dishes { get; } = new List<Dish>();

		public Category Id { get; set; }

		public MenuCategory(Category id)
		{
			this.Id = id;
			this.Name = id.ToString();
		}

        public void AddDish(Dish dish)
        {
            Dishes.Add(dish);
        }
    }
}
