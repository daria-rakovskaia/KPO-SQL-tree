using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace treeLab
{
    public partial class MainWindow : Window
    {
        static List<LoadedData> data = (List<LoadedData>)LoadedData.GetData();

        static string connectionString = @"Data Source=.\DEV;" +
                                         "Initial Catalog=Procedures;" +
                                         "Integrated Security=true;";

        public static TreeView myTree;

        public static MyTreeViewItem currItem;

        public MainWindow()
        {
            InitializeComponent();
            myTree = tree;
            MakeTree();
        }

        void MakeTree() // выгрузка данных и создание дерева
        {
            tree.Items.Clear();
            foreach (var item in new SortedSet<LoadedData>(data))
            {
                var country = new MyTreeViewItem(item.CountryId, item.Country, 0, null);
                country.Header = item.Country;
                tree.Items.Add(country);
                var makers = new SortedSet<int>();
                foreach (var maker in data)
                {
                    if (!makers.Contains(maker.MakerId) && maker.CountryId == item.CountryId)
                    {
                        var newMaker = new MyTreeViewItem(maker.MakerId, maker.Maker, 1, country);
                        newMaker.Header = maker.Maker;
                        country.Items.Add(newMaker);
                        makers.Add(maker.MakerId);
                        foreach (var vaccine in data)
                        {
                            if (vaccine.MakerId == maker.MakerId)
                            {
                                var newVaccine = new MyTreeViewItem(vaccine.VaccineId, vaccine.Vaccine, 2, newMaker);
                                newVaccine.Header = vaccine.Vaccine;
                                newMaker.Items.Add(newVaccine);
                            }
                        }
                    }
                }
            }
        }

        private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tree.SelectedItem == null)
            {
                delete.IsEnabled = false;
                add.IsEnabled = false;
                edit.IsEnabled = false;
            }
            else
            {
                delete.IsEnabled = true;
                add.IsEnabled = ((MyTreeViewItem)tree.SelectedItem).Level < 2;
                edit.IsEnabled = true;
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (MyTreeViewItem)tree.SelectedItem;
            string query = string.Empty;
            switch (selectedItem.Level)
            {
                case 0:
                    {
                        query = "DELETE from Countries where Countries.id = @id;";
                        tree.Items.Remove(selectedItem);
                        break;
                    }
                case 1:
                    {
                        query = "DELETE from VaccinesMakers where VaccinesMakers.id = @id;";
                        selectedItem.CustomParent.Items.Remove(selectedItem);
                        break;
                    }
                case 2:
                    {
                        query = "DELETE from Vaccines where Vaccines.id = @id;";
                        selectedItem.CustomParent.Items.Remove(selectedItem);
                        break;
                    }
            }
            var count = -1;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", selectedItem.Id);
                count = cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (MyTreeViewItem)tree.SelectedItem;
            switch (selectedItem.Level)
            {
                case 0:
                    {
                        currItem = selectedItem;
                        var addWndw = new AddMaker();
                        addWndw.ShowDialog();
                        break;
                    }
                case 1:
                    {
                        currItem = selectedItem;
                        var addWndw = new AddVaccine();
                        addWndw.ShowDialog();
                        break;
                    }
            }
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (MyTreeViewItem)tree.SelectedItem;
            switch (selectedItem.Level)
            {
                case 0:
                    {
                        currItem = selectedItem;
                        var editWndw = new EditCountry();
                        editWndw.ShowDialog();
                        break;
                    }
                case 1:
                    {
                        currItem = selectedItem;
                        var editWndw = new EditMaker();
                        editWndw.ShowDialog();
                        break;
                    }
                case 2:
                    {
                        currItem = selectedItem;
                        var editWndw = new EditVaccine();
                        editWndw.ShowDialog();
                        break;
                    }
            }
        }

        private void addCountry_Click(object sender, RoutedEventArgs e)
        {
            var addWndw = new AddCountry();
            addWndw.ShowDialog();
        }

        private void MyTreeViewItem_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(sender as DependencyObject, new DataObject(DataFormats.Serializable, sender), DragDropEffects.Move);
            }
        }

        private void TreeViewItem_Drop(object sender, System.Windows.DragEventArgs e)
        {
            RelocateItem(sender as TreeViewItem, e.Data.GetData(DataFormats.Serializable) as TreeViewItem);
        }

        private void RelocateItem(TreeViewItem newParent, TreeViewItem item)
        {
            var parent = item.Parent as TreeViewItem;
            parent.Items.Remove(item);
            newParent.Items.Add(item);
            ((MyTreeViewItem)item).CustomParent = (MyTreeViewItem)newParent;
        }
    }
}
