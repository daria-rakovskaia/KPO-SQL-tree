using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace treeLab
{
    public class MyTreeViewItem : TreeViewItem
    {
        string connectionString = @"Data Source=.\DEV;" +
                                  "Initial Catalog=Procedures;" +
                                  "Integrated Security=true;";

        public int Id { get; set; }
        
        public string Name { get; set; }

        public int Level { get; set; }

        public MyTreeViewItem CustomParent { get; set; }

        public MyTreeViewItem(int id, string name, int level, MyTreeViewItem customParent)
        {
            Id = id;
            Name = name;
            Level = level;
            CustomParent = customParent;
            if (Level > 0)
            {
                MouseMove += MyTreeViewItem_MouseMove;
            }
            Drop += MyTreeViewItem_Drop;
        }

        private void MyTreeViewItem_Drop(object sender, System.Windows.DragEventArgs e)
        {
            RelocateItem(sender as MyTreeViewItem, e.Data.GetData(DataFormats.Serializable) as MyTreeViewItem);
        }

        private void RelocateItem(MyTreeViewItem newParent, MyTreeViewItem item)
        {
            if (newParent.Level + 1 == item.Level && newParent != item.Parent)
            {
                var parent = item.Parent as TreeViewItem;
                parent.Items.Remove(item);
                newParent.Items.Add(item);
                item.CustomParent = newParent;
                switch (item.Level)
                {
                    case 1:
                        {
                            string query = "UPDATE VaccinesMakers SET CountryId = @countryId where VaccinesMakers.id = @id;";
                            using (var connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                var cmd = new SqlCommand(query, connection);
                                cmd.Parameters.AddWithValue("@countryId", newParent.Id);
                                cmd.Parameters.AddWithValue("@id", item.Id);
                                cmd.ExecuteNonQuery();
                                connection.Close();
                            }
                            break;
                        }
                    case 2:
                        {
                            string query = "UPDATE Vaccines SET MakerId = @makerId where Vaccines.id = @id;";
                            using (var connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                var cmd = new SqlCommand(query, connection);
                                cmd.Parameters.AddWithValue("@makerId", newParent.Id);
                                cmd.Parameters.AddWithValue("@id", item.Id);
                                cmd.ExecuteNonQuery();
                                connection.Close();
                            }
                            break;
                        }
                }
            }
        }

        private void MyTreeViewItem_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(sender as DependencyObject, new DataObject(DataFormats.Serializable, sender), DragDropEffects.Move);
            }
        }
    }
}
