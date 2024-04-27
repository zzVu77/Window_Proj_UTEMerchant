﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UTEMerchant
{
    /// <summary>
    /// Interaction logic for WinPlaceOrder.xaml
    /// </summary>
    public partial class WinPlaceOrder : Window
    {
        private readonly Dictionary<Seller, List<Item>> _items;
        private readonly User _user;
        private readonly double? _subTotalPrice;
        private double? _shippingFee;
        private double? _totalPrice;
        private bool _placeOrderSuccessful = false;


        public WinPlaceOrder()
        {
            InitializeComponent();
        }

        public WinPlaceOrder(Dictionary<Seller, List<Item>> items, User user, double subTotalPrice) : this()
        {
            this._items = items;
            this._subTotalPrice = subTotalPrice;
            _user = user;
        }

        public bool IsPlaceOrderComplete => _placeOrderSuccessful;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _placeOrderSuccessful = false;
            if (_subTotalPrice != null)
            {
                _totalPrice = _subTotalPrice;
                tbTotal.Text = "$" + _totalPrice;
            }

            if (_user != null) tbDeliveryAddress.Text = $"{_user.District}, {_user.Ward}, {_user.City}";
            if (_items != null)
            {
                foreach (KeyValuePair<Seller, List<Item>> entry in _items)
                {
                    AddItems(entry.Key, entry.Value);
                }
                tbSubTotal.Text = "$" + _subTotalPrice;
            }
            cbShippingChanel.SelectedValue = "Standard Shipping";
        }

        private void AddItems(Seller seller, List<Item> items)
        {
            UC_PlaceOrderItemsBox box = new UC_PlaceOrderItemsBox(seller);
            foreach (Item item in items)
            {
                box.AddItem(item);
            }

            if (spItems .Children.Count == 0)
            {
                spItems.Children.Add(box);
            }
            else if (spItems.Children.Count == 1)
            {
                if (spItems.Children[0] is UC_PlaceOrderItemsBox first) first.Margin = new Thickness(0, 0, 0, 10);
                box.Margin = new Thickness(0, 10, 0, 0);
                spItems.Children.Add(box);
            }
            else if (spItems.Children.Count > 1)
            {
                if (spItems.Children[spItems.Children.Count - 1] is UC_PlaceOrderItemsBox last) last.Margin = new Thickness(0,10,0,10);
                box.Margin = new Thickness(0, 10, 0, 0);
                spItems.Children.Add(box);
            }
        }

        private void cbShippingChanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbShippingChanel.SelectedIndex == 0)
            {
                _shippingFee = 5;
                tbShippingCost.Text = "$5.00";
                tbTotal.Text = "$" + CalculateTotalPrice().ToString("F");
            }
            else if (cbShippingChanel.SelectedIndex == 1)
            {
                _shippingFee = 10;
                tbShippingCost.Text = "$10.00";
                tbTotal.Text = "$" + CalculateTotalPrice().ToString("F");
            }
            else
            {
                _shippingFee = 0;
                tbShippingCost.Text = "$0.00";
                tbTotal.Text = "$" + CalculateTotalPrice().ToString("F");
            }
        }

        private double CalculateTotalPrice()
        {
            if (_subTotalPrice == null || _shippingFee == null) return 0;
            return (double)(_subTotalPrice + _shippingFee);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Check if the user has selected a shipping channel
            if (cbShippingChanel.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a shipping channel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if the user has entered details for delivery address
            if (tbDeliveryAddress.Text == "")
            {
                MessageBox.Show("Please enter your delivery address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                foreach (KeyValuePair<Seller, List<Item>> entry in _items)
                {
                    foreach (Item item in entry.Value)
                    {
                        new Item_DAO().UpdateStatus(item.Item_Id);
                        new PurchasedItem_DAO().AddItem(new purchasedItem()
                            { Id_user = _user.Id_user, Item_Id = item.Item_Id });
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while placing the order. Please try again later.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            finally
            {
                _placeOrderSuccessful = true;
                this.Close();
            }

        }
    }
}
