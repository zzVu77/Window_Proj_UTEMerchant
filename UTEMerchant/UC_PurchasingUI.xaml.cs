﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace UTEMerchant
{
    /// <summary>
    /// Interaction logic for UC_PurchasingUI.xaml
    /// </summary>
    public partial class UC_PurchasingUI : UserControl
    {
        private readonly Item_DAO _itemDao = new Item_DAO();
        private readonly Seller_DAO _sellerDao = new Seller_DAO();
        List<Item> _items = new List<Item>();
        private readonly ItemClicks_DAO _itemClick = new ItemClicks_DAO();
        private readonly ItemSearch_DAO _itemSearch =  new ItemSearch_DAO();
        public int IdUser { get; set; }


        public UC_PurchasingUI()
        {
            InitializeComponent();
        }

        private void XIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSearchBox.Text = "";
        }

        private void btnRelevance_Click(object sender, RoutedEventArgs e)
        {
            if (btnRelevance.Background == Brushes.Transparent)
            {
                btnPopular.Background = Brushes.Transparent;
                btnPrice.Background = Brushes.Transparent;
                btnTrend.Background = Brushes.Transparent;
                wpItemsList.Children.Clear();
                List<Item> items = _itemDao.SortRevelance(IdUser);

                List<Item> sortedItems = _items.OrderByDescending(item => item.Item_Id).ToList();
                foreach (Item item in sortedItems)
                {
                    AddItem(item);
                }
                btnRelevance.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#EAAC8B");
            }
            else
            {
                btnRelevance.Background = Brushes.Transparent;
                wpItemsList.Children.Clear();
                foreach (Item item in _items)
                {
                    AddItem(item);
                }
            }
        }

        private void btnPopular_Click(object sender, RoutedEventArgs e)
        {
            List<Item> items = _items;
            if (btnPopular.Background == Brushes.Transparent)
            {

                btnRelevance.Background = Brushes.Transparent;
                btnPrice.Background = Brushes.Transparent;
                btnTrend.Background = Brushes.Transparent;

                wpItemsList.Children.Clear();
                List<ItemClicks> itemClicks = _itemClick.Load();
                List<Item> sortedItems = items.OrderByDescending(item =>
                {
                    var itemClick = itemClicks.FirstOrDefault(click => click.Item_Id == item.Item_Id);
                    return itemClick != null ? itemClick.Click_Count : 0;
                }).ToList();
                foreach (Item item in sortedItems)
                {
                    UC_ItemView uc_item = new UC_ItemView(item);
                    uc_item.MouseLeftButtonDown += wpItemsList_MouseLeftButtonDown;
                    wpItemsList.Children.Add(uc_item);
                }
                btnPopular.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#EAAC8B");
            }
            else
            {
                btnRelevance.Background = Brushes.Transparent;
                wpItemsList.Children.Clear();
                foreach (Item item in _items)
                {
                    UC_ItemView uc_item = new UC_ItemView(item);
                    uc_item.MouseLeftButtonDown += wpItemsList_MouseLeftButtonDown;
                    wpItemsList.Children.Add(uc_item);
                }
                btnPopular.Background = Brushes.Transparent;
            }
        }

        private void btnPrice_Click(object sender, RoutedEventArgs e)
        {
            List<Item> items = _items;
            if (btnPrice.Background == Brushes.Transparent)
            {
                btnRelevance.Background = Brushes.Transparent;
                btnPopular.Background = Brushes.Transparent;
                btnTrend.Background = Brushes.Transparent;

                btnPrice.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#EAAC8B");
                wpItemsList.Children.Clear();
                List<Item> Sorteditems = items.OrderByDescending(item => item.Price).ToList();
                foreach (Item item in Sorteditems)
                {
                    AddItem(item);
                }

            }
            else
            {
                wpItemsList.Children.Clear();
                foreach (Item item in _items)
                {
                    AddItem(item);
                }
                btnPrice.Background = Brushes.Transparent;
                
            }
        }

        private void imgFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dpRightSideBar.Visibility == Visibility.Collapsed)
            {
                dpRightSideBar.Visibility = Visibility.Visible;
            }
            else dpRightSideBar.Visibility = Visibility.Collapsed;
        }


        private void OnItemButtonAddToCartClicked(object sender, RoutedEventArgs e)
        {
            // The button in UC_ItemView was clicked!
            if (sender is UC_ItemView clickedItemView)
            {
                Item clickedItem = clickedItemView.info;

                // Find existing seller view, otherwise create a new one
                var sellerView = uc_ShoppingCart.spItems.Children.OfType<UC_ShoppingCartItemsView>()
                    .FirstOrDefault(seller => seller.GetSeller().SellerID == clickedItem.SellerID);

                if (sellerView == null)
                {
                    sellerView = new UC_ShoppingCartItemsView(_sellerDao.GetSeller(clickedItem.SellerID));
                    uc_ShoppingCart.spItems.Children.Add(sellerView);
                }

                // Check if the item is already in the cart
                if (sellerView.spItems.Children.OfType<UC_ItemInShoppingCartItemsView>()
                    .Any(item => item.GetItem().Item_Id == clickedItem.Item_Id))
                {
                    return;
                }
                sellerView.AddItem(clickedItem);
                sellerView.spItems.Children.OfType<UC_ItemInShoppingCartItemsView>().Last().TogItemChecked += RecalculateTotalPrice;
                sellerView.spItems.Children.OfType<UC_ItemInShoppingCartItemsView>().Last().TogItemUnchecked += RecalculateTotalPrice;
                uc_ShoppingCart.CheckCart();
            }
        }

        private void wpItemsList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UC_ItemView clickedItem)
            {
                Seller seller = _sellerDao.GetSeller(clickedItem.info.SellerID);
                _itemClick.UpdateClick(clickedItem.info.Item_Id);
                WinDeltailItem winDeltailItem = new WinDeltailItem(clickedItem.info, seller, IdUser);
                winDeltailItem.ShowDialog();
                //add to cart
                if(winDeltailItem.check)
                {
                    OnItemButtonAddToCartClicked(sender, e);
                }  
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            UserControl_Loaded(this, new RoutedEventArgs());
        }

        public void imgShoppingCart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            grdPurchasingInterface.IsEnabled = false;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            grdPurchasingInterface.IsEnabled = true;
        }


        private void uc_ShoppingCart_Loaded(object sender, RoutedEventArgs e)
        {
            uc_ShoppingCart.CheckCart();
        }

        private void RecalculateTotalPrice(object sender, RoutedEventArgs e)
        {
            if (sender is UC_ItemInShoppingCartItemsView item)
            {
                double total = Double.Parse(tbTotalPriceValue.Text);

                if (item.togItem.IsChecked == true)
                {
                    total += item.GetItem().Price;
                }
                else
                {
                    total -= item.GetItem().Price;
                }

                tbTotalPriceValue.Text = total.ToString(CultureInfo.CurrentCulture);
            }
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<Seller, List<Item>> items = new Dictionary<Seller, List<Item>>();
            foreach (UC_ShoppingCartItemsView sellerView in uc_ShoppingCart.spItems.Children.OfType<UC_ShoppingCartItemsView>())
            {
                if (sellerView.GetSelectedItems().Count == 0) continue;
                items.Add(sellerView.GetSeller(), sellerView.GetSelectedItems());
            }

            WinPlaceOrder winPlaceOrder = new WinPlaceOrder(items, double.Parse(tbTotalPriceValue.Text));
            winPlaceOrder.ShowDialog();

            if (winPlaceOrder.IsPlaceOrderComplete)
            {
                uc_ShoppingCart.spItems.Children.Clear();
                tbTotalPriceValue.Text = "0";

                UserControl_Loaded(this, new RoutedEventArgs());
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Activate the loaded event when the user control is visible
            if (IsVisible)
            {
                UserControl_Loaded(sender, new RoutedEventArgs());
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            wpItemsList.Children.Clear();
            if (StaticValue.SELLER != null) _items = _itemDao.Load().Where(i => i.SellerID != StaticValue.SELLER.SellerID).ToList(); else _items = _itemDao.Load();
            _items.Sort((item1, item2) => item1.Sale_Status.CompareTo(item2.Sale_Status));
            foreach (Item item in _items)
            {
                AddItem(item);
            }
        }

        private void AddItem(Item item)
        {
            UC_ItemView uc_item = new UC_ItemView(item);
            uc_item.ItemClicked += OnItemButtonAddToCartClicked;
            uc_item.MouseLeftButtonDown += wpItemsList_MouseLeftButtonDown;
            wpItemsList.Children.Add(uc_item);
        }


        private void txtSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtSearchBox.Text))
                {
                    List<Item> items = _itemDao.Search(txtSearchBox.Text, StaticValue.SELLER.SellerID);
                    if (items.Count > 0)
                    {
                        wpItemsList.Children.Clear();
                        foreach (Item item in items)
                        {
                         
                            _itemSearch.UpdateSearch(item.Item_Id, IdUser);
                            AddItem(item);
                        }
                    }
                }
                else
                {
                    wpItemsList.Children.Clear();
                    foreach (Item item in _items)
                    {
                        AddItem(item);
                    }
                }
            }
        }

        private void imgSearchIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearchBox.Text))
            {
                List<Item> items = _itemDao.Search(txtSearchBox.Text, StaticValue.SELLER.SellerID);
                if (items.Count > 0)
                {
                    wpItemsList.Children.Clear();
                    foreach (Item item in items)
                    {
                        _itemSearch.UpdateSearch(item.Item_Id, IdUser);
                        UC_ItemView uc_item = new UC_ItemView(item);
                        uc_item.ItemClicked += OnItemButtonAddToCartClicked;
                        uc_ShoppingCart.CheckCart();
                        uc_item.MouseLeftButtonDown += wpItemsList_MouseLeftButtonDown;
                        wpItemsList.Children.Add(uc_item);
                    }
                }
            }
            else
            {
                wpItemsList.Children.Clear();
                foreach (Item item in _items)
                {
                    UC_ItemView uc_item = new UC_ItemView(item);
                    uc_item.ItemClicked += OnItemButtonAddToCartClicked;
                    uc_ShoppingCart.CheckCart();
                    uc_item.MouseLeftButtonDown += wpItemsList_MouseLeftButtonDown;
                    wpItemsList.Children.Add(uc_item);
                }
            }
        }

        private void btnTrend_Click(object sender, RoutedEventArgs e)
        {
            List<Item> items = _items;
            if (btnTrend.Background == Brushes.Transparent)
            {
                wpItemsList.Children.Clear();
                btnRelevance.Background = Brushes.Transparent;
                btnPrice.Background = Brushes.Transparent;
                btnPopular.Background = Brushes.Transparent;
                List<ItemSearch> itemSearchs = _itemSearch.LoadbyUser(IdUser);
                List<Item> sortedItems = items.OrderByDescending(item =>
                {
                    var itemClick = itemSearchs.FirstOrDefault(click => click.Item_Id == item.Item_Id);
                    return itemClick != null ? itemClick.Search_Count : 0;
                }).ToList();
                foreach (Item item in sortedItems)
                {
                    UC_ItemView uc_item = new UC_ItemView(item);
                    uc_item.MouseLeftButtonDown += wpItemsList_MouseLeftButtonDown;
                    wpItemsList.Children.Add(uc_item);
                }
                btnTrend.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#EAAC8B");
            }
            else
            {

                wpItemsList.Children.Clear();
                foreach (Item item in _items)
                {
                    UC_ItemView uc_item = new UC_ItemView(item);
                    uc_item.MouseLeftButtonDown += wpItemsList_MouseLeftButtonDown;
                    wpItemsList.Children.Add(uc_item);
                }
                btnTrend.Background = Brushes.Transparent;
            }
        }
    }
}
