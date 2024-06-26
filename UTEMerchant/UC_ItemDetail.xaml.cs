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
    /// Interaction logic for UC_ItemDetail.xaml
    /// </summary>
    public partial class UC_ItemDetail : UserControl
    {
        Item info;
        List<User> users = new List<User>();
        user_DAO user_dao = new user_DAO();
        public UC_ItemDetail()
        {
            InitializeComponent();
            SetDefaultText();
        }
        public UC_ItemDetail(Item item)
        {
            info = item;
            var user_dao= new user_DAO();
            users = user_dao.Load();
            InitializeComponent();
            SetDefaultText();
        }
        private void SetDefaultText()
        {
            txblItemName.Text= info.Name;
            txbOriginalPrice.Text = info.Original_Price.ToString()+" $";
            txblBoughtDate.Text = info.Bought_date.ToString();

            foreach (User user in users) 
            {
                if (user.Id_user == info.SellerID)
                {
                    txblContactValue.Text = user.Phone;
                    break;
                }
            }
            txblItemPrice.Text = info.Price.ToString()+" $";
            txblTypeValue.Text = info.Type.ToString();
            txblStatus.Text = info.Condition.ToString();
            var resourceUri = new Uri(info.Image_Path, UriKind.RelativeOrAbsolute);
            imgItemPic.Source = new BitmapImage(resourceUri);
            // Tạo một FlowDocument
            FlowDocument flowDoc = new FlowDocument();
            // Thêm một Paragraph chứa văn bản mặc định vào FlowDocument
            Paragraph paragraph = new Paragraph(new Run(info.Condition_Description));
            flowDoc.Blocks.Add(paragraph);
            // Gán FlowDocument cho RichTextBox
            rtbDescription.Document = flowDoc;
        }
    }
}
