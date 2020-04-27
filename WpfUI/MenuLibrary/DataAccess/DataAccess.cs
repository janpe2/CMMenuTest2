using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Dapper;
using System.Data;

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
            using (IDbConnection connection = 
                   new System.Data.SqlClient.SqlConnection(CnnVal(CurrentDBName)))
            {
                var list = connection.Query<Dish>("dbo.spDish_GetByName @Name", new { Name = name }).AsList();
                //var list = connection.Query<Dish>($"select * from Dish where Name='{name}'").AsList();

                return list.Count == 0 ? null : list[0];
            } 
        }

        public Dish GetDishById(int id)
        {
            using (IDbConnection connection =
                   new System.Data.SqlClient.SqlConnection(CnnVal(CurrentDBName)))
            {
                var list = connection.Query<Dish>("dbo.spDish_GetById @Id", new { ID = id }).AsList();
                return list.Count == 0 ? null : list[0];
            }
        }

        public List<Dish> GetAllDishes()
        {
            using (IDbConnection connection =
                   new System.Data.SqlClient.SqlConnection(CnnVal(CurrentDBName)))
            {
                List<Dish> list = connection.Query<Dish>("dbo.Dish_GetAllDishesSortedAlphabetically").AsList();
                //List<Dish> list = connection.Query<Dish>("select * from Dish").AsList();
                return list;
            }
        }

        public void InsertDish(Dish dish)
        {
            using (IDbConnection connection =
                   new System.Data.SqlClient.SqlConnection(CnnVal(CurrentDBName)))
            {
                List<Dish> list = new List<Dish>();
                list.Add(dish);

                //connection.Execute($"insert into Dish (Name, Description) values ('{name}', '{descr}')");
                connection.Execute(
                    "dbo.Dish_Insert @Name, @Description, @Price, @ContainsLactose, @ContainsGluten, @ContainsFish",
                    list);
            }
        }

        public void DeleteDish(Dish dish)
        {
            using (IDbConnection connection =
                   new System.Data.SqlClient.SqlConnection(CnnVal(CurrentDBName)))
            {
                connection.Execute("dbo.Dish_Delete @Id", new { Id = dish.Id });
            }
        }

        public void ModifyDish(Dish dish)
        {
            using (IDbConnection connection =
                   new System.Data.SqlClient.SqlConnection(CnnVal(CurrentDBName)))
            {
                List<Dish> list = new List<Dish>();
                list.Add(dish);

                connection.Execute(
                    "dbo.Dish_Modify @Id, @Name, @Description, @Price, @ContainsLactose, @ContainsGluten, @ContainsFish",
                    list);
            }
        }

        public List<Menu> GetAllMenus()
        {
            throw new NotImplementedException();
        }

        public void AddMenu(Menu menu)
        {
            throw new NotImplementedException();
        }

        public void DeleteMenu(Menu menu)
        {
            throw new NotImplementedException();
        }
    }
}
