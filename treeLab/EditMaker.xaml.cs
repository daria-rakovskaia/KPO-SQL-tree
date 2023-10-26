using Accessibility;
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
    /// Логика взаимодействия для EditMaker.xaml
    /// </summary>
    public partial class EditMaker : Window
    {
        internal class VaccineMaker
        {
            internal string Name { get; set; }

            internal int CountryId { get; set; }

            internal VaccineMaker(string name, int countryId)
            {
                Name = name;
                CountryId = countryId;
            }
        }

        public EditMaker()
        {
            InitializeComponent();
            FillCountries();
            maker.Text = MainWindow.currItem.Name;
        }

        string connectionString = @"Data Source=.\DEV;" +
                                  "Initial Catalog=Procedures;" +
                                  "Integrated Security=true;";

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            var name = maker.Text.Trim();
            if (name == "")
                MessageBox.Show("Ввод неверный!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string query = "select id from Countries where Countries.Name = @name;";
                int id = -1;
                var count = -1;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@name", countries.SelectedValue.ToString());
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                        id = reader.GetInt32(0);
                    reader.Close();

                    var currVaccinesMakers = new List<VaccineMaker>();
                    query = @"select VaccinesMakers.Name, VaccinesMakers.CountryId from VaccinesMakers";
                    cmd = new SqlCommand(query, connection);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                        currVaccinesMakers.Add(new VaccineMaker(reader.GetString(0).Trim(), reader.GetInt32(1)));
                    reader.Close();

                    bool isExists = false;
                    foreach (var vm in currVaccinesMakers)
                    {
                        if (vm.Name == name && vm.CountryId == id)
                        {
                            isExists = true;
                            break;
                        }
                    }
                    
                    if (!isExists)
                    {
                        query = "UPDATE VaccinesMakers SET Name = @name, CountryId = @countryId where VaccinesMakers.id = @id;";
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@countryId", id);
                        cmd.Parameters.AddWithValue("@id", MainWindow.currItem.Id);
                        count = cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                if (count == -1)
                    MessageBox.Show("Такой производитель уже есть!", "Ошибка при изменении",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                {
                    MainWindow.currItem.CustomParent.Items.Remove(MainWindow.currItem);
                    var currCountries = MainWindow.myTree.Items;
                    foreach (MyTreeViewItem c in currCountries)
                    {
                        if (c.Name == countries.SelectedValue.ToString())
                        {
                            var currMaker = new MyTreeViewItem(MainWindow.currItem.Id, name, 1, c);
                            currMaker.Header = name;
                            c.Items.Add(currMaker);
                            break;
                        }    
                    }
                    MessageBox.Show("Производитель изменен", "", MessageBoxButton.OK);
                    this.Close();
                }
            }
        }

        private void FillCountries()
        {
            var currCountries = MainWindow.myTree.Items;
            foreach (MyTreeViewItem c in currCountries)
                countries.Items.Add(c.Name);
            countries.SelectedValue = MainWindow.currItem.CustomParent.Name;
        }
    }
}
