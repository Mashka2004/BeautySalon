using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for SchedulesAdd.xaml
    /// </summary>
    public partial class SchedulesAdd : Window
    {
        public SchedulesAdd()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand($@"Select es.schedule_id, e.first_name As 'Имя', e.last_name As 'Фамилия', `date` As 'Дата', start_time As 'Начало рабочего дня', end_time As 'Конец рабочего дня' From Employee_Schedules es
                                                                inner join Employees e  on e.employee_id = es.employee_id where es.schedule_id = '{viewBase.MyData.schedules_id}'", con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    Employee.Text=$"{dt.Rows[0].ItemArray[1]} {dt.Rows[0].ItemArray[2]}";
                    datePicker1.Text=dt.Rows[0].ItemArray[3].ToString();
                    start_time.Text = dt.Rows[0].ItemArray[4].ToString();
                    end_time.Text = dt.Rows[0].ItemArray[5].ToString();
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
        private void datePicker1_CalendarOpened(object sender, RoutedEventArgs e)
        {

        }
    }
}
