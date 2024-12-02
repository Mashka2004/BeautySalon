using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using BeautySalon.viewBase;
using MySql.Data.MySqlClient;

namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for OrdersAdd.xaml
    /// </summary>
    public partial class OrdersAdd : Window
    { 
        public OrdersAdd()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
          


            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                try
                {
                    con.Open();

                    DateTime selectedData = datePicker1.SelectedDate.Value;
                    string data = selectedData.ToString("yyyy-MM-dd");
                    string time = Time.Text;
                    string dataTime = $"{data} {time}";

                    string[] clients = Client.Text.Split('-');
                    string[] services = Services.Text.Split('-');
                    string client_id = clients[0];
                    string services_id = services[0];
                    string status = "На исполнении";

                    List<int> MaterialsQuery = new List<int>();
                    List<int> Quantity = new List<int>();
                    // Создание команды для проверки количества материалов на складе, связанных с определенной услугой
                    using (MySqlCommand checkMaterialsQuery = new MySqlCommand($@"
                        SELECT quantity_in_stock
                        FROM Materials m
                        INNER JOIN Service_Materials s ON m.material_id = s.material_id
                        WHERE s.service_id = '{services_id}'", con))
                    {
                        using (MySqlDataReader dr = checkMaterialsQuery.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                MaterialsQuery.Add(dr.GetInt32(0));
                            }
                        }
                    }
                    // Создание команды для проверки использованного количества материалов для определенной услуги
                    using (MySqlCommand checkQuantity_usedQuery = new MySqlCommand($@"
                        SELECT quantity_used
                        FROM VKR.Service_Materials
                        WHERE service_id = '{services_id}'", con))
                    {

                        using (MySqlDataReader dr2 = checkQuantity_usedQuery.ExecuteReader())
                        {
                            while (dr2.Read())
                            {
                                Quantity.Add(dr2.GetInt32(0));
                            }
                        }
                    }
                    for (int i = 0; i < MaterialsQuery.Count; i++)
                    {
                        // Если количество материалов на складе минус использованное количество меньше 0,
                        // то выводим сообщение об ошибке и выходим из метода
                        if (MaterialsQuery[i] - Quantity[i] < 0)
                        {
                            MessageBox.Show("На складе недостаточно продуктов");
                            return;
                        }
                    }

                    // Создание команды для вставки новой записи о выполненной услуге в таблицу Recording
                    using (MySqlCommand cmd = new MySqlCommand($@"
                            INSERT INTO VKR.Recording (client_id, service_id, recording_datetime, status)
                            VALUES ('{client_id}','{services_id}', '{dataTime}', '{status}')", con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    // Создание команды для обновления количества материалов на складе после выполнения услуги
                    using (MySqlCommand cmd2 = new MySqlCommand($@"
                                UPDATE Materials m
                                JOIN Service_Materials s ON m.material_id = s.material_id
                                SET m.quantity_in_stock = m.quantity_in_stock - s.quantity_used
                                WHERE s.service_id = '{services_id}'", con))
                    {
                        cmd2.ExecuteNonQuery();
                    }
                 
                    MessageBox.Show("Запись записана");

                    Time.Text = "";
                    Client.Text = "";
                    datePicker1.Text = "";
                    Services.Text = "";

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка-" + ex.Message);
                }
            }
        }

  
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                try
                {
                    con.Open();
                    MySqlCommand cmd1 = new MySqlCommand("Select * From Clients",con);

                    MySqlDataReader dr1 = cmd1.ExecuteReader();

                    while (dr1.Read())
                    {
                        Client.Items.Add($"{dr1[0]}-{dr1[1]} {dr1[2]}");
                    }
                    con.Close();

                    con.Open();
                    MySqlCommand cmd2 = new MySqlCommand("Select * From Services", con);

                    MySqlDataReader dr2 = cmd2.ExecuteReader();
                    while (dr2.Read())
                    {
                        Services.Items.Add($"{dr2[0]}-{dr2[1]}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка-" + ex.Message);
                }
            }
        }
        private void datePicker1_CalendarOpened(object sender, RoutedEventArgs e)
        {
            if (datePicker1!=null)
            {
                DateTime minDate = DateTime.Now;
                DateTime maxDate = DateTime.Now.AddMonths(1);

                if (datePicker1.SelectedDate < minDate || datePicker1.SelectedDate > maxDate)
                {
                    datePicker1.SelectedDate = null; 
                }
            }
        }

     
    }
}