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
        private int currentPage = 1;
        private const int pageSize = 15;
        private int totalRecords;
        private void UpdatePaginationButtons(int totalPages)

        {
            PaginationBar.Children.Clear();
            for (int i = 0; i < (int)Math.Ceiling((double)totalRecords / pageSize); i++)
            {
                var paginationBtn = new Button
                {
                    Width = 30,
                    Height = 30,
                    Style = (Style)FindResource("BtnStyle"),
                    Content = (i + 1).ToString(),
                    Margin = new Thickness(0, 0, 10, 0)
                };
                paginationBtn.Click += PaginationBtn_Click;
                PaginationBar.Children.Add(paginationBtn);
        }
        }
        private int GetTotalCount(List<string> filters)
        {
            string countQuery = @"SELECT COUNT(*) FROM Cosmetic_Products";

            // Если есть условия фильтрации, добавляем их к запросу
            if (filters.Count > 0)
            {
                countQuery += " WHERE " + string.Join(" AND ", filters);
            }

            using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                con.Open();
                using (MySqlCommand countCommand = new MySqlCommand(countQuery, con))
                {
                    totalRecords = Convert.ToInt32(countCommand.ExecuteScalar());
                    return totalRecords;
                }
            }
        }
  
            // Начальное SQL-запрос
            private void filteringAndSorting()
            {
                // Начальное SQL-запрос
                query = @"SELECT product_id, product_name AS 'Наименование продукта',
                            type AS 'Тип', description AS 'Описание', price AS 'Цена', 
                            quantity_in_stock AS 'Кол-во на складе', image FROM Cosmetic_Products";

                string sortOrder = "";
                List<string> filters = new List<string>(); // Список для хранения условий фильтрации

                // Сортировка по выбранному значению из ComboBox1
                if (ComboBox1.SelectedItem != null)
                {
                    string selectedSortValue = (ComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();
                    switch (selectedSortValue)
                    {
                        case "По Возврастанию":
                            sortOrder += " ORDER BY product_name ASC";
                            break;
                        case "По Убыванию":
                            sortOrder += " ORDER BY product_name DESC";
                            break;
                    }
                }

                // Фильтрация по типу, выбранному в ComboBox2
                if (ComboBox2.SelectedItem != null)
                {
                    string selectedTypeValue = (ComboBox2.SelectedItem as ComboBoxItem)?.Content.ToString();
                    filters.Add($"type = '{selectedTypeValue}'");
                }

                // Получение текста для поиска
                string filterText = searchBox.Text.Trim();

                // Добавляем фильтрацию по имени продукта
                if (!string.IsNullOrEmpty(filterText))
                {
                    filters.Add($"product_name LIKE '%{filterText}%'");
                }

                // Если есть условия фильтрации, добавляем их к запросу
                if (filters.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", filters);
                }

                // Получение общего количества записей для пагинации
                int totalCount = GetTotalCount(filters);

                // Определяем количество страниц
                int pageSize = 15; // Количество записей на странице
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Обновляем кнопки пагинации
                UpdatePaginationButtons(totalPages);

                // Добавляем сортировку к запросу
                if (!string.IsNullOrEmpty(sortOrder))
                {
                    query += sortOrder;
                }

                // Обновляем DataGrid с новыми данными
                UpdateGrid(query, 1);
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
            UpdateGrid(query, currentPage);


            if (MyData.role =="Администратор")
            {
                AddBtn.Visibility = Visibility.Collapsed;
                DellBtn.Visibility = Visibility.Hidden;
               
            }
            for (int i = 0; i < (int)Math.Ceiling((double)totalRecords / pageSize); i++)
            {
                var paginationBtn = new Button
                {
                    Width = 30,
                    Height = 30,
                    Style = (Style)FindResource("BtnUC"),
                    Content = (i + 1).ToString(),
                    Margin = new Thickness(0, 0, 10, 0)
                };
                paginationBtn.Click += PaginationBtn_Click;
                PaginationBar.Children.Add(paginationBtn);
            }
            }


        private  Button  selectedPaginationButton;
        private void PaginationBtn_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (selectedPaginationButton != null)
            {
                selectedPaginationButton.Style = (Style)FindResource("BtnUC");
            }

            clickedButton.Style = (Style)FindResource("BtnStyleActive");
            selectedPaginationButton = clickedButton;

            currentPage = int.Parse(clickedButton.Content.ToString());
            UpdateGrid(query, currentPage);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            ProductAdd ProductAdd = new ProductAdd();
            ProductAdd.ShowDialog();
            FormUtils.workTable.Opacity = 1;
            this.Opacity = 1;
            UpdateGrid(query, currentPage);
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
            UpdateGrid(query,currentPage);
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
        }
        private void UpdateGrid(string query, int page)
        {
            query += $@" Limit {(page - 1) * pageSize}, {pageSize}";
            DataTable dataTable = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection);
                connection.Open();
                string countQuery = "SELECT COUNT(*) FROM Cosmetic_Products";

                MySqlCommand countCommand = new MySqlCommand(countQuery, connection);

                totalRecords = Convert.ToInt32(countCommand.ExecuteScalar());
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
                UpdateGrid(query, currentPage);
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
            currentPage = 1;
            UpdateGrid(query, currentPage); 
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
        }

        private void searchBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        private void Btn_Click_1(object sender, RoutedEventArgs e)
        {
            int maxPage = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Проверяем, что текущая страница меньше максимальной
            if (currentPage < maxPage)
            {
                currentPage += 1;
                UpdateGrid(query, currentPage);
            }
        }

        private void Btn_Click_2(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage -= 1;
                UpdateGrid(query, currentPage);
            }
        }
    }
}