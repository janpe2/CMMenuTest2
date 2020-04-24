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

        public Dish GetDish(string name)
        {
            using (IDbConnection connection = 
                   new System.Data.SqlClient.SqlConnection(CnnVal(CurrentDBName)))
            {
                var list = connection.Query<Dish>("dbo.spDish_GetByName @Name", new { Name = name }).AsList();

                // Toimii
                //var list = connection.Query<Dish>($"select * from Dish where Name='{name}'").AsList();

                return list.Count == 0 ? null : list[0];
            } 
        }

        public List<Dish> GetAllDishes()
        {
            using (IDbConnection connection =
                   new System.Data.SqlClient.SqlConnection(CnnVal(CurrentDBName)))
            {
                List<Dish> list = connection.Query<Dish>($"select * from Dish").AsList();
                return list;
            }
        }

        public void InsertDish(string name, string descr, double price)
        {
            using (IDbConnection connection =
                   new System.Data.SqlClient.SqlConnection(CnnVal(CurrentDBName)))
            {
                List<Dish> list = new List<Dish>();
                list.Add(new Dish(name, descr, price));

                connection.Execute($"insert into Dish (Name, Description) values ('{name}', '{descr}')");

                //connection.Execute("dbo.Dish_Insert @Name, @Description, @Price, @ContainsLactose, @ContainsGluten, @ContainsFish", list);

            }
        }
    }
}
