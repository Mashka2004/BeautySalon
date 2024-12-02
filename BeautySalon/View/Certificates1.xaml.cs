using BeautySalon.viewBase;
using MySql.Data.MySqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for Certificates1.xaml
    /// </summary>
    public partial class Certificates1 : UserControl
    {
        public Certificates1()
        {
            InitializeComponent();
        }
        private readonly string FileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Template", "template.docx");
        Word.Document wordDocument;
        string id;
        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                id = selectedRow[0].ToString();
                Print.IsEnabled = true;
            }
            else
            {

                MyData.clients_id = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                var wordApp = new Word.Application();
                try
                {
                    wordDocument = wordApp.Documents.Open(FileName);
                }
                catch
                {
                    String path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "template.docx");
                    var wordDocument = wordApp.Documents.Open(FileName);
                }
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand($"SELECT * FROM VKR.Certificates where certificate_id = '{id}'",con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);


                    string certificate_number = dt.Rows[0].ItemArray[1].ToString();
                    string amount = dt.Rows[0].ItemArray[2].ToString();
                    string expiration_date = dt.Rows[0].ItemArray[3].ToString();

                    wordApp.Visible = false;

                    try
                    {
                     
                        ReplaceWordStub("{certificate_number}", certificate_number, wordDocument);
                        ReplaceWordStub("{amount}", amount, wordDocument);
                        ReplaceWordStub("{expiration_date}", expiration_date, wordDocument);


                        wordApp.Visible = true;
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }                  
                }
            }
        }
        private void ReplaceWordStub(string stubToReplace, string text, Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Print.IsEnabled = false;
            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
                {
                    con.Open();

                    string query = $@"Select certificate_id,certificate_number As 'Номер сертификат', amount As 'Стоимость', issue_date As 'Дата начала',
                                      expiration_date 'Дата окончания' From Certificates";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView.ItemsSource = dt.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }

            dataGridView.Columns[0].Visibility = Visibility.Collapsed;
        }
    }
}
