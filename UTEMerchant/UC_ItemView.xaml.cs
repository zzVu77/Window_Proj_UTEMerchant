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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UTEMerchant
{
    /// <summary>
    /// Interaction logic for UC_ItemView.xaml
    /// </summary>
    public partial class UC_ItemView : UserControl
    {
        public event EventHandler ItemClicked;

        public Item info;
        
        public UC_ItemView()
        {
            InitializeComponent();
            this.MouseUp += UC_ItemView_MouseUp;
            
        }
        public UC_ItemView(Item item) : this()
        {            
            info = item;
            SetDefaultValue();
        }

        private void UC_ItemView_MouseUp(object sender, MouseButtonEventArgs e)
        {            
            ItemClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            ItemClicked?.Invoke(this, EventArgs.Empty);
        }

        public void SetDefaultValue()
        {
            txblItemName.Text = info.Name;
            txblPrice.Text = info.Price.ToString()+"$";
            txblOldPrice.Text=info.OriginalPrice.ToString()+"$";
            txblCondition.Text = info.Status.ToString() + "%";
            var resourceUri = new Uri(info.ImagePath, UriKind.RelativeOrAbsolute);
            imgItemPic.Source = new BitmapImage(resourceUri);


        }
    }

}

