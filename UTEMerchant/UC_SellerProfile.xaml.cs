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
    /// Interaction logic for UC_SellerProfile.xaml
    /// </summary>
    public partial class UC_SellerProfile : UserControl
    {
        public event EventHandler SavedButtonClicked;
        //Seller seller = new Seller();
        //User user = new User();
        string selectedCity;
        string selectedDistrict;
        Seller_DAO seller_dao = new Seller_DAO();
        user_DAO user_dao = new user_DAO();
        Address_DAO address_dao = new Address_DAO();
        List<Address> distinctCities;
        public string image_path;
        private const string ImgRelativePath = "../../Img/";

        private static readonly string ExecutablePath = AppDomain.CurrentDomain.BaseDirectory;

        private static readonly string ImgFilePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(ExecutablePath, ImgRelativePath));
        public UC_SellerProfile()
        {
            InitializeComponent();
        }
        public void SetDefault()
        {
            //this.seller = seller;
            //this.user = user;
            distinctCities = address_dao.Load();
            cbPickupCity.Items.Clear();
            foreach (Address address in distinctCities)
            {
                cbPickupCity.Items.Add(address.City);
            }
            txtSellerShopName.Text = StaticValue.SELLER.ShopName;
            txtSellerID.Text = StaticValue.SELLER.Id_user.ToString();
            txtSellerShopEmail.Text = StaticValue.USER.Email;
            txtSellerPhoneNumber.Text = StaticValue.SELLER.Phone.ToString();
            txtSellerCity.Text = StaticValue.SELLER.City;
            txtSellerDistrict.Text = StaticValue.SELLER.District;
            txtSellerWard.Text = StaticValue.SELLER.Ward;
            image_path = System.IO.Path.Combine(ImgFilePath, StaticValue.USER.Image_Path);
            var resourceUri = new Uri(image_path, UriKind.RelativeOrAbsolute);
            imgSellerPhoto.Source = new BitmapImage(resourceUri);

        }

        private void ChangeTextBoxBackGroundEdit()
        {
            txtSellerShopName.Background = Brushes.White;
            txtSellerPhoneNumber.Background = Brushes.White;
            txtSellerShopEmail.Background = Brushes.White;
            txtSellerWard.Background = Brushes.White;

            txtSellerShopName.IsReadOnly = false;
            txtSellerPhoneNumber.IsReadOnly = false;
            txtSellerShopEmail.IsReadOnly = false;
            txtSellerWard.IsReadOnly = false;
        }
        private void ChangeTextBoxBackGroundSave()
        {
            txtSellerShopName.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f4f4f4"));
            txtSellerPhoneNumber.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f4f4f4"));
            txtSellerShopEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f4f4f4"));
            txtSellerWard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f4f4f4"));

            btnSave.Visibility = Visibility.Collapsed;
            txtSellerShopName.IsReadOnly = true;
            txtSellerPhoneNumber.IsReadOnly = true;
            txtSellerShopEmail.IsReadOnly = true;
            txtSellerWard.IsReadOnly = true;
            cbPickupCity.Visibility = Visibility.Collapsed;
            cbPickupDistrict.Visibility = Visibility.Collapsed;
            txtSellerCity.Visibility = Visibility.Visible;
            txtSellerDistrict.Visibility = Visibility.Visible;
        }
        private void cbPickupCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPickupCity.SelectedItem != null)
            {
                selectedCity = cbPickupCity.SelectedItem.ToString();
                txtSellerCity.Text = selectedCity;

                // Filter districts based on selected city
                var filteredDistricts = address_dao.getdistrict(selectedCity);

                // Update district ComboBox
                if (cbPickupCity.Items != null)
                    cbPickupDistrict.Items.Clear();
                foreach (var district in filteredDistricts)
                {
                    cbPickupDistrict.Items.Add(district);
                }
                cbPickupDistrict.SelectedIndex = -1;
            }
        }

        private void cbPickupDistrict_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPickupDistrict.SelectedItem != null)
            {
                selectedDistrict = cbPickupDistrict.SelectedItem.ToString();
                txtSellerDistrict.Text = selectedDistrict;
            }
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            cbPickupCity.Visibility = Visibility.Visible;
            txtSellerCity.Visibility = Visibility.Collapsed;
            cbPickupDistrict.Visibility = Visibility.Visible;
            txtSellerDistrict.Visibility = Visibility.Collapsed;

            txtSellerShopName.IsReadOnly = false;
              txtSellerShopEmail.IsReadOnly= false;
            txtSellerPhoneNumber.IsReadOnly= false;
                   txtSellerWard.IsReadOnly= false;

            ChangeTextBoxBackGroundEdit();

            for (int i = 0; i < distinctCities.Count; i++)
            {
                if (distinctCities[i].City == txtSellerCity.Text)
                {
                    cbPickupCity.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < distinctCities.Count; i++)
            {
                if (distinctCities[i].District == txtSellerDistrict.Text)
                {
                    cbPickupDistrict.SelectedIndex = i; //Chỗ này bị lỗi tại trong combobox của District chỉ có mỗi Nha Trang, Fix lại chỗ này sau
                    break;
                }
            }

            btnSave.Visibility = Visibility.Visible;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            /*string selectedValue = cbPickupCity.SelectedItem.ToString();
            txtSellerCity.Text = selectedValue;

            selectedValue = cbPickupDistrict.SelectedItem.ToString();
            txtSellerDistrict.Text = selectedValue;
*/
            txtSellerShopName.IsReadOnly = true;
            txtSellerShopEmail.IsReadOnly = true;
            txtSellerPhoneNumber.IsReadOnly = true;
            txtSellerWard.IsReadOnly = true;

            ChangeTextBoxBackGroundSave();

            cbPickupCity.Visibility = Visibility.Collapsed;
            txtSellerCity.Visibility = Visibility.Visible;
            cbPickupDistrict.Visibility = Visibility.Collapsed;
            txtSellerDistrict.Visibility = Visibility.Visible;

            btnSave.Visibility = Visibility.Collapsed;

            StaticValue.SELLER.SellerID = Int32.Parse(txtSellerID.Text);
            if (string.IsNullOrWhiteSpace(txtSellerShopName.Text) || string.IsNullOrWhiteSpace(txtSellerShopEmail.Text) ||
               string.IsNullOrWhiteSpace(txtSellerWard.Text) || string.IsNullOrWhiteSpace(txtSellerPhoneNumber.Text)
               )
            {
                MessageBox.Show("Please complete all information");
                txtSellerShopName.Text = StaticValue.SELLER.ShopName;
                txtSellerShopEmail.Text = StaticValue.USER.Email;
                txtSellerPhoneNumber.Text = StaticValue.SELLER.Phone;
                txtSellerWard.Text = StaticValue.SELLER.Ward;
            }
            else
            {
                StaticValue.SELLER.ShopName = txtSellerShopName.Text;
                StaticValue.USER.Email = txtSellerShopEmail.Text;
                if (!string.IsNullOrEmpty(selectedCity))
                    StaticValue.SELLER.City = selectedCity;
                else StaticValue.SELLER.City = txtSellerCity.Text;
                if (!string.IsNullOrEmpty(selectedDistrict))
                    StaticValue.SELLER.District = selectedDistrict;
                else StaticValue.SELLER.District = txtSellerDistrict.Text;
                StaticValue.SELLER.Ward = txtSellerWard.Text;
                StaticValue.SELLER.Phone = txtSellerPhoneNumber.Text;

                StaticValue.USER.Image_Path = image_path;
                seller_dao.UpdateSeller(StaticValue.SELLER);
                user_dao.UpdateUserThroughSeller(StaticValue.USER);
                SavedButtonClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        private void btnChangePhoto_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Images (*.jpg,*.png)|*.jpg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                image_path = selectedFilePath;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(image_path);
                bitmap.EndInit();
                imgSellerPhoto.Source = bitmap;
                // Xử lý đường dẫn đã chọn ở đây
            }
            btnSave.Visibility = Visibility.Visible;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                ChangeTextBoxBackGroundSave();
            }
        }
    }
}
