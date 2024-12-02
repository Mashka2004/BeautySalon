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
using System.Windows.Shapes;
using BeautySalon.viewBase;
using BeautySalon.View;
namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for workTable.xaml
    /// </summary>
    public partial class workTable : Window
    {
        public workTable()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вернуться на форму авторизации?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            role.Content = MyData.role;
            Name.Content = MyData.name;

            switch (MyData.role) {

                case "Администратор":
                    Materials.Visibility = Visibility.Collapsed;
                    Employees.Visibility = Visibility.Collapsed;
                    break;

                case "Мастер":
                    Employees.Visibility = Visibility.Collapsed;
                    Clients.Visibility = Visibility.Collapsed;
                    Services.Visibility = Visibility.Collapsed;
                    Materials.Visibility = Visibility.Collapsed;
                    Products.Visibility = Visibility.Collapsed;
                    Certificates.Visibility = Visibility.Collapsed;
                    break;

                case "Директор":
                    /**/
                    break;

                case "Менеджер":
                    Employees.Visibility = Visibility.Collapsed;
                    Clients.Visibility = Visibility.Collapsed;
                    Orders.Visibility = Visibility.Collapsed;
                    Schedules.Visibility = Visibility.Collapsed;
                    Services.Visibility = Visibility.Collapsed;
                    Certificates.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        private void Employees_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            StackPanelActive.Children.Clear();
            switch (button.Name)
            {
                case "Employees":
                    Employees employees = new Employees();
                    StackPanelActive.Children.Add(employees);
                    break;
                case "Materials":
                    Materials materials = new Materials();
                    StackPanelActive.Children.Add(materials);
                    break;
                case "Orders":
                    Orders orders = new Orders();
                    StackPanelActive.Children.Add(orders);
                    break;
                case "Services":
                    Services services = new Services();
                    StackPanelActive.Children.Add(services);
                    break;
                case "Products":
                    Product product = new Product();
                    StackPanelActive.Children.Add(product);
                    break;
                case "Clients":
                    Clients Clients = new Clients();
                    StackPanelActive.Children.Add(Clients);
                    break;
                case "Schedules":
                    Schedules schedules = new Schedules();
                    StackPanelActive.Children.Add(schedules);
                    break;
                case "Certificates":
                    Certificates1 certificates = new Certificates1();
                    StackPanelActive.Children.Add(certificates);
                    break;
            }
        }
    }
}