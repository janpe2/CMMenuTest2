using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace WpfUI.MenuLibrary.DataAccess
{
    public class DataAccess
    {
        public static string CurrentDBName = "MenuDBConnection";

        public static string CnnVal(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public Dish GetDishByName(string name)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                var list = connection.Query<Dish>("dbo.spDish_GetByName @Name", new { Name = name }).AsList();
                //var list = connection.Query<Dish>($"select * from Dish where Name='{name}'").AsList();

                return list.Count == 0 ? null : list[0];
            } 
        }

        public Dish GetDishById(int id)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                var list = connection.Query<Dish>("dbo.Dish_GetById @Id", new { Id = id }).AsList();
                return list.Count == 0 ? null : list[0];
            }
        }

        public List<Dish> GetAllDishes()
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                List<Dish> list = connection.Query<Dish>("dbo.Dish_GetAllDishesSortedAlphabetically").AsList();
                //List<Dish> list = connection.Query<Dish>("select * from Dish").AsList();
                return list;
            }
        }

        public void InsertDish(Dish dish)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                connection.Execute(
                    "dbo.Dish_Insert @Name, @Description, @Price, @ContainsLactose, @ContainsGluten, @ContainsFish",
                    dish);
            }
        }

        public void DeleteDish(Dish dish)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                // Remove the dish from all menus first to avoid a SqlException
                connection.Execute($"delete from DishesInCategory where DishId='{dish.Id}'");

                connection.Execute("dbo.Dish_Delete @Id", new { Id = dish.Id });
            }
        }

        public void ModifyDish(Dish dish)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                List<Dish> list = new List<Dish>();
                list.Add(dish);

                connection.Execute(
                    "dbo.Dish_Modify @Id, @Name, @Description, @Price, @ContainsLactose, @ContainsGluten, @ContainsFish",
                    list);
            }
        }

        public Menu GetMenuById(int menuId)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                List<Menu> list = connection.Query<Menu>($"select * from Menu where Id='{menuId}'").AsList();
                return list.Count == 0 ? null : list[0];
            }
        }

        public List<Menu> GetAllMenus()
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                List<Menu> list = connection.Query<Menu>("select * from Menu").AsList();
                return list;
            }
        }

        public List<Dish> GetDishesInCategory(int categoryId, int menuId)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                /*
                List<int> dishIds = connection.Query<int>(
                    $"select DishId from DishesInCategory where CategoryId='{categoryId}' and MenuId='{menuId}'").AsList();
                var dishes = connection.Query<Dish>("select * from Dish where Id in @ids", new { ids = dishIds });
                return dishes.AsList();
                */
                
                
                return connection.Query<Dish>("dbo.DishesInCategory_GetDishes @CategoryId, @MenuId",
                    new { CategoryId = categoryId, MenuId = menuId }).AsList();
                
            }
        }

        public void AddMenu(Menu menu)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                connection.Execute($"insert into Menu (Name, Description) values ('{menu.Name}', '{menu.Description}')");
            }
        }

        public void DeleteMenu(Menu menu)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                // Remove the menu from DishesInCategory first to avoid a SqlException
                connection.Execute($"delete from DishesInCategory where MenuId='{menu.Id}'");

                connection.Execute($"delete from Menu where Id='{menu.Id}'");
            }
        }

        public void AddDishToCategory(int dishId, int category, int menuId)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                connection.Execute("dbo.DishesInCategory_Insert @DishId, @CategoryId, @MenuId", 
                    new { DishId = dishId, CategoryId = category, MenuID = menuId });
            }
        }

        public void RemoveDishFromCategory(int dishId, int categoryId, int menuId)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                connection.Execute(
                    $"delete from DishesInCategory where DishId='{dishId}' and CategoryId='{categoryId}' and MenuId='{menuId}'");
            }
        }

        public void ModifyMenu(Menu menu, string newName, string newDescr)
        {
            using (IDbConnection connection = new SqlConnection(CnnVal(CurrentDBName)))
            {
                connection.Execute(
                    $"update Menu set Name='{newName}', Description='{newDescr}' where Id='{menu.Id}'");
            }
        }

    }
}
