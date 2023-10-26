using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;

namespace treeLab
{
    /// <summary>
    /// Логика взаимодействия для EditCountry.xaml
    /// </summary>
    public partial class EditCountry : Window
    {
        public EditCountry()
        {
            InitializeComponent();
            country.Text = MainWindow.currItem.Name;
        }

        string connectionString = @"Data Source=.\DEV;" +
                                  "Initial Catalog=Procedures;" +
                                  "Integrated Security=true;";

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex("[0-9]+");
            var name = country.Text.Trim();
            if (regex.IsMatch(name) || name.Trim() == "")
                MessageBox.Show("Ввод неверный!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string query = $"UPDATE Countries SET Name = @name where id = @id;";
                int id = MainWindow.currItem.Id;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                MainWindow.currItem.Name = name;
                MainWindow.currItem.Header = name;
                MessageBox.Show("Страна изменена", "", MessageBoxButton.OK);
                this.Close();
            }
        }
    }
}
