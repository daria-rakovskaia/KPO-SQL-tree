using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace treeLab
{
    /// <summary>
    /// Логика взаимодействия для EditVaccine.xaml
    /// </summary>
    public partial class EditVaccine : Window
    {
        internal class Vaccine
        {
            internal string Name { get; set; }

            internal int MakerId { get; set; }

            internal Vaccine(string name, int makerId)
            {
                Name = name;
                MakerId = makerId;
            }
        }

        public EditVaccine()
        {
            InitializeComponent();
            vaccine.Text = MainWindow.currItem.Name.Trim();
            FillMakers();
        }

        string connectionString = @"Data Source=.\DEV;" +
                                  "Initial Catalog=Procedures;" +
                                  "Integrated Security=true;";

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            var name = vaccine.Text.Trim();
            if (name == "")
                MessageBox.Show("Ввод неверный!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string query = "select id from VaccinesMakers where VaccinesMakers.Name = @name;";
                int id = -1;
                var count = -1;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@name", makers.SelectedValue.ToString());
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                        id = reader.GetInt32(0);
                    reader.Close();

                    var currVaccines = new List<Vaccine>();
                    query = @"select Vaccines.Name, Vaccines.MakerId from Vaccines";
                    cmd = new SqlCommand(query, connection);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                        currVaccines.Add(new Vaccine(reader.GetString(0).Trim(), reader.GetInt32(1)));
                    reader.Close();

                    bool isExists = false;
                    foreach (var v in currVaccines)
                    {
                        if (v.Name == name && v.MakerId == id)
                        {
                            isExists = true;
                            break;
                        }
                    }

                    if (!isExists)
                    {
                        query = "UPDATE Vaccines SET Name = @name, MakerId = @makerId, TypeId = 29 where Vaccines.id = @id;";
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@makerId", id);
                        cmd.Parameters.AddWithValue("@id", MainWindow.currItem.Id);
                        count = cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                if (count == -1)
                    MessageBox.Show("Такая вакцина уже есть!", "Ошибка при изменении",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                {
                    MainWindow.currItem.CustomParent.Items.Remove(MainWindow.currItem);
                    var currVaccinesMakers = new List<MyTreeViewItem>();
                    foreach (MyTreeViewItem c in MainWindow.myTree.Items)
                    {
                        foreach (MyTreeViewItem vm in c.Items)
                            currVaccinesMakers.Add(vm);
                    }

                    foreach (MyTreeViewItem vm in currVaccinesMakers)
                    {
                        if (vm.Name == makers.SelectedValue.ToString())
                        {
                            var currVaccine = new MyTreeViewItem(MainWindow.currItem.Id, name, 2, vm);
                            currVaccine.Header = name;
                            vm.Items.Add(currVaccine);
                            break;
                        }
                    }
                    MessageBox.Show("Вакцина изменена", "", MessageBoxButton.OK);
                    this.Close();
                }
            }
        }

        private void FillMakers()
        {
            foreach (MyTreeViewItem c in MainWindow.myTree.Items)
            {
                foreach (MyTreeViewItem vm in c.Items)
                    makers.Items.Add(vm.Name.Trim());
            }
            makers.SelectedValue = MainWindow.currItem.CustomParent.Name.Trim();
        }
    }
}
