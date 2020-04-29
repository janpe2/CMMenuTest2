using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.MenuLibrary
{
    public class MenuCategory
    {
		public static string[] CategoryNames = Enum.GetNames(typeof(Menu.Category));

		/// <summary>
		/// Name of the category.
		/// </summary>
		public string Name { get; set; } = "";

		/// <summary>
		/// Describes this category.
		/// </summary>
		public string Description { get; set; } = "";

		/// <summary>
		/// Dishes that belong to the category.
		/// </summary>
		public List<Dish> Dishes { get; } = new List<Dish>();

		public int Id { get; set; }

		public Menu.Category CatId { get; set; }

		public MenuCategory(Menu.Category id)
		{
			this.CatId = id;
			this.Id = (int)id;
			this.Name = id.ToString();
		}

		public MenuCategory(Menu.Category id, Dish dish) :
			this(id)
		{
			AddDish(dish);
		}

		public void AddDish(Dish dish)
        {
            Dishes.Add(dish);
        }
    }
}
