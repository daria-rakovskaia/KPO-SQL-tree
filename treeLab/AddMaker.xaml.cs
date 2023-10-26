using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace treeLab
{
    /// <summary>
    /// Логика взаимодействия для AddMaker.xaml
    /// </summary>
    public partial class AddMaker : Window
    {
        public AddMaker()
        {
            InitializeComponent();
            country.Content += ' ' + MainWindow.currItem.Name;
        }

        string connectionString = @"Data Source=.\DEV;" +
                                  "Initial Catalog=Procedures;" +
                                  "Integrated Security=true;";

        private void add_Click(object sender, RoutedEventArgs e)
        {
            var name = maker.Text.Trim();
            if (name == "")
                MessageBox.Show("Ввод неверный!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                bool isExists = false;
                var makers = MainWindow.currItem.Items;
                foreach (MyTreeViewItem m in makers)
                {
                    if (m.Name == name)
                    {
                        isExists = true;
                        break;
                    }
                }
                if (!isExists)
                {
                    string query = $"INSERT into VaccinesMakers values('{name}', {MainWindow.currItem.Id});";
                    int id = -1;
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var cmd = new SqlCommand(query, connection);
                        var count = cmd.ExecuteNonQuery();
                        query = "select id from VaccinesMakers where VaccinesMakers.Name = @name;";
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@name", name);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            id = reader.GetInt32(0);
                        connection.Close();
                        reader.Close();
                    }
                    var newMaker = new MyTreeViewItem(id, name, 1, MainWindow.currItem);
                    newMaker.Header = name;
                    MainWindow.currItem.Items.Add(newMaker);
                    MessageBox.Show("Производитель добавлен в базу данных", "", MessageBoxButton.OK);
                    this.Close();
                }
                else
                    MessageBox.Show("Такой производитель уже есть!", "Ошибка при добавлении",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
