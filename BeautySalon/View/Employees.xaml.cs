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
using MySql.Data.MySqlClient;
using System.Windows.Shapes;
using BeautySalon.viewBase;
using System.Data;
using BeautySalon.Forms ;

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for Employees.xaml
    /// </summary>
    public partial class Employees : UserControl
    {
        public Employees()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            UpdateGrid();
            DellBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
        }
        private void UpdateGrid()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
                {
                    con.Open();

                    string query = @"SELECT employee_id, first_name AS 'Имя', last_name AS 'Фамилия', patronymic AS 'Отчество', login AS 'Логин', password AS 'Пароль', 
                                   role AS 'Роль' FROM Employees";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView.ItemsSource = dt.DefaultView;

                        dataGridView.Columns[0].Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            EmployeesAdd EmployeesAdd = new EmployeesAdd();
            EmployeesAdd.ShowDialog();
            FormUtils.workTable.Opacity = 1;
            UpdateGrid();
            this.Opacity = 1;
            DellBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
        }

        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                MyData.employee_id = selectedRow[0].ToString();
                EditBtn.IsEnabled = true;
                DellBtn.IsEnabled = true;
            }
            else
            {
                MyData.orders_id = null;
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            EmployesEdit EmployesEdit = new EmployesEdit();
            EmployesEdit.ShowDialog();
            FormUtils.workTable.Opacity = 1;
            UpdateGrid();
            this.Opacity = 1;
            DellBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
        }

        private void DellBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить данного сотрудника?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand($"DELETE FROM VKR.employee_schedules WHERE employee_id = '{MyData.employee_id}';", con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (MySqlCommand cmd = new MySqlCommand($"DELETE FROM VKR.Employees WHERE employee_id = '{MyData.employee_id}';", con))
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Пользователь удалён");
                    }

                    EditBtn.IsEnabled = false;
                    DellBtn.IsEnabled = false;
                    UpdateGrid();
                }
            }
        }
    }
}