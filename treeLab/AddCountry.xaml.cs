using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;

namespace treeLab
{
    /// <summary>
    /// Логика взаимодействия для AddCountry.xaml
    /// </summary>
    public partial class AddCountry : Window
    {
        public AddCountry()
        {
            InitializeComponent();
        }

        string connectionString = @"Data Source=.\DEV;" +
                                  "Initial Catalog=Procedures;" +
                                  "Integrated Security=true;";

        private void add_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex("[0-9]+");
            var name = country.Text.Trim();
            if (regex.IsMatch(name) || name.Trim() == "")
                MessageBox.Show("Ввод неверный!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string query = "select Countries.Name from Countries;";
                var countries = new List<string>();
                using (var connect = new SqlConnection(connectionString))
                {
                    connect.Open();
                    var cmd = new SqlCommand(query, connect);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                        countries.Add(reader.GetString(0).Trim());
                    reader.Close();
                    connect.Close();
                }
                bool isExists = false;
                foreach (var c in countries)
                {
                    if (c == name)
                    {
                        isExists = true;
                        break;
                    }
                }
                if (!isExists)
                {
                    query = $"INSERT into Countries values('{name}');";
                    int id = -1;
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var cmd = new SqlCommand(query, connection);
                        var count = cmd.ExecuteNonQuery();
                        query = "select id from Countries where Countries.Name = @name;";
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@name", name);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            id = reader.GetInt32(0);
                        connection.Close();
                    }
                    var newCountry = new MyTreeViewItem(id, name, 0, null);
                    newCountry.Header = name;
                    MainWindow.myTree.Items.Add(newCountry);
                    MessageBox.Show("Страна добавлена в базу данных", "", MessageBoxButton.OK);
                    this.Close();
                }
                else
                    MessageBox.Show("Такая страна уже есть!", "Ошибка при добавлении", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
