using BeautySalon.Forms;
using BeautySalon.viewBase;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class Product : UserControl
    {
        public string query = @"SELECT product_id, product_name As 'Наименование продукта',
                                type As 'Тип', description As 'Описание', price As 'Цена', 
                                quantity_in_stock As 'Кол-во на складе', image From Cosmetic_Products";
        public Product()
        {
            InitializeComponent();
        }

        private void filteringAndSorting()
        {
            query = @"SELECT product_id, product_name As 'Наименование продукта',
                                type As 'Тип', description As 'Описание', price As 'Цена', 
                                quantity_in_stock As 'Кол-во на складе', image From Cosmetic_Products";

            string sortOrder = "";

            if (ComboBox1.SelectedItem != null)
            {
                string selectedSortValue = (ComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();
                switch (selectedSortValue)
                {
                    case "По Возврастанию":
                        sortOrder += "ORDER BY product_name ASC";
                        break;
                    case "По Убыванию":
                        sortOrder += "ORDER BY product_name DESC";
                        break;

              
                }
            }
            if (ComboBox2.SelectedItem != null)
            {
                string selectedTypeValue = (ComboBox2.SelectedItem as ComboBoxItem)?.Content.ToString();
              
                    query += $" WHERE type = '{selectedTypeValue}'";

              
             
            }

            string filterText = searchBox.Text.Trim();
            if (!string.IsNullOrEmpty(filterText))
            {
                if (ComboBox2.SelectedItem != null )
                {
                    query += $" AND product_name LIKE '%{filterText}%'";
                }
                else
                {
                    query += $" WHERE product_name LIKE '%{filterText}%'";
                }
            }
            if (sortOrder != null)
            {
                query += " " + sortOrder;
            }

            UpdateGrid(query);
        }
        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
         

            filteringAndSorting();
        }
        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)ComboBox1.Template.FindName("textBlock", ComboBox1);

            if (ComboBox1.SelectedItem ==null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
            filteringAndSorting();
        }
        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)ComboBox2.Template.FindName("textBlock", ComboBox2);

            if (ComboBox2.SelectedItem == null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
            filteringAndSorting();
        }
        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                MyData.products_id = selectedRow[0].ToString();
                EditBtn.IsEnabled = true;
                DellBtn.IsEnabled = true;
            }
            else
            {

                MyData.products_id = null;
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
            UpdateGrid(query);


            if (MyData.role =="Администратор")
            {
                AddBtn.Visibility = Visibility.Collapsed;
                DellBtn.Visibility = Visibility.Hidden;
               
            }
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            ProductAdd ProductAdd = new ProductAdd();
            ProductAdd.ShowDialog();
            FormUtils.workTable.Opacity = 1;
            this.Opacity = 1;
            UpdateGrid(query);
                EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
        }
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            ProductEdit ProductEdit = new ProductEdit();
            ProductEdit.ShowDialog();
            FormUtils.workTable.Opacity = 1;
            this.Opacity = 1;
            UpdateGrid(query);
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
        }
        private void UpdateGrid(string query)
        {
            DataTable dataTable = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection);
                connection.Open();
                try
                {
                    dataAdapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при извлечении данных: {ex.Message}");
                }
            }
            dataGridView.ItemsSource = dataTable.DefaultView;
            dataGridView.Columns[0].Visibility = Visibility.Collapsed;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                con.Open();

                var result = MessageBox.Show("Вы действительно хотите удалить данный товар?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                    using (MySqlCommand cmd = new MySqlCommand($"DELETE FROM VKR.orders_cosmetic_products WHERE product_id = '{MyData.products_id}'", con))
                {
                    cmd.ExecuteNonQuery();
              
                }

                using (MySqlCommand cmd = new MySqlCommand($"DELETE FROM VKR.Cosmetic_Products WHERE product_id = '{MyData.products_id}'", con))
                {
                    cmd.ExecuteNonQuery();

                }
                MessageBox.Show("Товар удалён");
                EditBtn.IsEnabled = false;
                DellBtn.IsEnabled = false;
                UpdateGrid(query);
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.SelectedItem = null;
            ComboBox2.SelectedItem = null;
            searchBox.Text = string.Empty;
            query = @"SELECT product_id, product_name As 'Наименование продукта',
                                type As 'Тип', description As 'Описание', price As 'Цена', 
                                quantity_in_stock As 'Кол-во на складе', image From Cosmetic_Products";
            UpdateGrid(query); EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
        }

        private void searchBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }
    }
}