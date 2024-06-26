﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTEMerchant
{
    public class Seller: Person
    {
        public int SellerID { get; set; }
        public int Id_user { get; set; }

        public string ShopName { get; set; }
      
        public double Average_rating { get; set; }
      
        public Seller()
        {
            
        }
        public Seller(int sellerID, int id_user, string shopName, string city, string district, string ward,double average_rating, string phone)
        {
            SellerID = sellerID;
            Id_user = id_user;
            ShopName = shopName;
            City = city;
            District = district;
            Ward = ward;
            Average_rating = average_rating;
            Phone = phone;
        }
        public Seller( int id_user, string shopName, string city, string district, string ward, string phone)
        {            
            Id_user = id_user;
            ShopName = shopName;
            City = city;
            District = district;
            Ward = ward;
            Phone = phone;
        }
    }
}
