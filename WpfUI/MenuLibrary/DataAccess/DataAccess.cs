using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Dapper;

namespace WpfUI.MenuLibrary.DataAccess
{
    public class DataAccess
    {
        public static string CnnVal(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public string GetJotain(string name)
        {
            using (System.Data.IDbConnection connection = 
                   new System.Data.SqlClient.SqlConnection(CnnVal("MenuDBConnection")))
            {
                var enumer = connection.Query<Dish>($"select * from Dish where Name='{name}'");
                List<Dish> list = enumer.AsList();
                return list.Count == 0 ? "empty" : list[0].Name;
            } 
        }
    }
}
