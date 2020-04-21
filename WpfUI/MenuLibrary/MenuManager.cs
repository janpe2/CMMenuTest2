using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.MenuLibrary
{
    public class MenuManager
    {
        public List<Dish> AllDishes { get; set; } = new List<Dish>();

        public List<MenuCategory> AllCategories { get; set; } = new List<MenuCategory>()
        {
            new MenuCategory(Menu.Category.Starter),
            new MenuCategory(Menu.Category.MainCourse),
            new MenuCategory(Menu.Category.Dessert),
            new MenuCategory(Menu.Category.Drink),
            new MenuCategory(Menu.Category.Uncategorized),
        };
        

        public List<Menu> AllMenus { get; set; } = new List<Menu>();

        public void LoadSampleData()
        {
            Menu menu1 = new Menu("Lounasmenu", "Tarjolla keskipäivällä");
            AllMenus.Add(menu1);
            AddDish(Menu.Category.Starter, "Kalakeitto", "Perunaa ja haukea", 6.55, menu1, true, false, true);
            AddDish(Menu.Category.MainCourse, "Pippuripihvi", "Pippurilla maustettu naudan pihvi", 16.95, menu1, false, false, false);
            AddDish(Menu.Category.MainCourse, "Lehtipihvi", "Ohut pihvi ja kastiketta", 10.85, menu1, false, false, false);
            AddDish(Menu.Category.MainCourse, "Kanafilee", "Mureaa kanan fileetä ja riisiä", 11.05, menu1, false, false, false);
            AddDish(Menu.Category.Dessert, "Suklaakakku", "Suklaista kakkua ja kermavaahtoa", 7.45, menu1, true, true, false);
            //menu1.AddDrink("Vesi", "Puhdasta vettä", 1.50, 0.0);
            //menu1.AddDrink("Olut", "Hienoa olutta", 4.95, 4.7);

            Menu menu2 = new Menu("A la carte -menu", "Tilauksen mukaan");
            AllMenus.Add(menu2);
            AddDish(Menu.Category.Starter, "Haukisalaattia", "Ruotoista haukea ja rehuja", 10.65, menu2, false, false, true);
            AddDish(Menu.Category.MainCourse, "Pekonipihvi", "Naudan pihvi ja pekonia", 17.95, menu2, false, false, false);
            AddDish(Menu.Category.Dessert, "Jäätelöä", "Kylmää jäätelöä", 9.72, menu2, true, false, false);
        }

        /*
        internal Menu.Category GetCategoryOfDish(Dish dish)
        {
            // TODO This doesn't work if a dish can exist in more than one categories.

            foreach (var cat in AllCategories)
            {
                if (cat.Dishes.Contains(dish))
                {
                    return cat.Id;
                }
            }
            return Menu.Category.Starter; // TODO
        }
        */

        public Dish AddDish(Menu.Category category, string name, string descr, double price, Menu menu,
            bool lactose, bool gluten, bool fish)
        {
            Dish dish = new Dish(name, descr, price) { ContainsLactose = lactose, ContainsGluten = gluten, ContainsFish = fish };

            AllDishes.Add(dish);
            //AllCategories[(int)category].AddDish(dish);
            menu.Categories[(int)category].AddDish(dish);

            return dish;
        }

        public List<Dish> GetAllDishesSortedAlphabetically()
        {
            List<Dish> list = new List<Dish>(AllDishes);
            list.Sort((a, b) => a.Name.CompareTo(b.Name));
            return list;
        }

    }
}
