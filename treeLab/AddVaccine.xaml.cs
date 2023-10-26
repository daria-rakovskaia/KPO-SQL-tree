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
    /// Логика взаимодействия для AddVaccine.xaml
    /// </summary>
    public partial class AddVaccine : Window
    {
        public AddVaccine()
        {
            InitializeComponent();
            maker.Content += ' ' + MainWindow.currItem.Name;
        }

        string connectionString = @"Data Source=.\DEV;" +
                                  "Initial Catalog=Procedures;" +
                                  "Integrated Security=true;";

        private void add_Click(object sender, RoutedEventArgs e)
        {
            var name = vaccine.Text.Trim();
            if (name == "")
                MessageBox.Show("Ввод неверный!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                bool isExists = false;
                var vaccines = MainWindow.currItem.Items;
                foreach (MyTreeViewItem v in vaccines)
                {
                    if (v.Name == name)
                    {
                        isExists = true;
                        break;
                    }
                }
                if (!isExists)
                {
                    string query = $"INSERT into Vaccines values('{name}', {MainWindow.currItem.Id}, 29);";
                    int id = -1;
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var cmd = new SqlCommand(query, connection);
                        var count = cmd.ExecuteNonQuery();
                        query = "select id from Vaccines where Vaccines.Name = @name;";
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@name", name);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            id = reader.GetInt32(0);
                        connection.Close();
                    }
                    var newVaccine = new MyTreeViewItem(id, name, 2, MainWindow.currItem);
                    newVaccine.Header = name;
                    MainWindow.currItem.Items.Add(newVaccine);
                    MessageBox.Show("Вакцина добавлена в базу данных", "", MessageBoxButton.OK);
                    this.Close();
                }
                else
                    MessageBox.Show("Такая вакцина уже есть!", "Ошибка при добавлении",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
