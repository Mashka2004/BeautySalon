using System;
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
using MySql.Data.MySqlClient;
using BeautySalon.viewBase;
using System.Data;
using BeautySalon.Forms;

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for Clients.xaml
    /// </summary>
    public partial class Clients : UserControl
    {
        public Clients()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGrid();

            if (MyData.role == "Администратор")
            {
            
            }
            else
            {
                EditBtn.IsEnabled = false;

            }
        }
        private void UpdateGrid()
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                try
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"Select client_id,first_name As 'Имя', last_name As 'Фамилия',
                                                          phone As 'Телефон', email As 'Почта', registration_date As 'Дата регистрации' From Clients", con);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView.ItemsSource = dt.DefaultView;
                    dataGridView.Columns[0].Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка - {ex.Message}");
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            ClientsAdd clientsAdd = new ClientsAdd();
            clientsAdd.ShowDialog();
            FormUtils.workTable.Opacity = 1;
            UpdateGrid();
            EditBtn.IsEnabled = false;
            this.Opacity = 1;
        }
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            ClientsEdit ClientsEdit = new ClientsEdit();
            ClientsEdit.ShowDialog();
            FormUtils.workTable.Opacity = 1;
            UpdateGrid();
            EditBtn.IsEnabled = false;
            this.Opacity = 1;
        }

        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                EditBtn.IsEnabled = true;
            
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                MyData.clients_id = selectedRow[0].ToString();
                EditBtn.IsEnabled = true;
            }
            else
            {

                MyData.clients_id = null;
            }
        }
    }
}