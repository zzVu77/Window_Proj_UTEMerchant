﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTEMerchant
{
    internal class Seller_DAO : DAO<Seller>
    {
        
    
        public override List<Seller> Load()
        {
            return db.LoadData<Seller>("SELECT * FROM [dbo].[Seller]");
        }
        public Seller GetSeller(int sellerID)
        {
            Seller seller = null;
            string sqlStr = "Select  * From  [dbo].[Seller] Where @sellerID = [dbo].[Seller].SellerID ";
            SqlConnection conn = new SqlConnection(db.connectionString);
            conn.Open();
            SqlCommand command = new SqlCommand(sqlStr, conn);
            command.Parameters.AddWithValue("@sellerID", sellerID);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                int SellerID = reader.GetInt32(0);
                int Id_user = reader.GetInt32(1);
                string ShopName = reader.GetString(2);
                string City = reader.GetString(3);
                string District = reader.GetString(4);
                string Ward = reader.GetString(5);
                string Phone = reader.GetString(6);
                double Average_rating = reader.GetDouble(7);
                return seller = new Seller(SellerID, Id_user, ShopName, City, District, Ward, Average_rating, Phone);
            }

            return seller;
        }
    }
}
