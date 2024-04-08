﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UTEMerchant
{
    public class DB_Connection
    {
        public string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\db_ute_merchant.mdf;Integrated Security=True";

        public DB_Connection() { } // Removed empty constructor

        public void ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddRange(parameters);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<T> LoadData<T>(string tableName) where T : new()
        {
            List<T> items = new List<T>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand($"SELECT * FROM [dbo].[{tableName}]", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T item = Activator.CreateInstance<T>();
                            LoadItemProperties(reader, item);
                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        private void LoadItemProperties<T>(SqlDataReader reader, T item)
        {
            // Reflection-based property loading
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string columnName = property.Name.ToLower(); // Assuming column names match property names
                try
                {
                    if (columnName != "image")
                    {
                        int columnIndex = reader.GetOrdinal(columnName); // Check if column exists
                        object value = reader[columnIndex]; // Get value from the column
                        property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    
                }
                catch (Exception ex)
                {
                  
                }
            }
        }

    }
}
