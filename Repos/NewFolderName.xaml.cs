using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Repos
{
    /// <summary>
    /// Interaction logic for NewFolderName.xaml
    /// </summary>
    public partial class NewFolderName : Window
    {
        public NewFolderName()
        {
            InitializeComponent();
            New.Focus();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (New.Text.Length > 0)
            {
                if (New.Text != (string)(Owner as MainWindow).Repos.SelectedItem)
                {
                    Directory.CreateDirectory(@"C:\Repos\" + New.Text);

                    foreach (FileInfo item in (Owner as MainWindow).WorkWithDirectory.FileInfos)
                        File.Move(item.FullName, @"C:\Repos\" + New.Text + @"\" + item.Name);

                    (Owner as MainWindow).WorkWithDirectory.DirectoryInfo.Delete();
                }
                else
                    MessageBox.Show("Хранилище с таким названием уже существует", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);

                Close();
            }
            else
            {
                MessageBox.Show("Введите навзание хранилища", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Error);
                New.Focus();
            }
        }

        private void New_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.IsMatch(New.Text, @"^[^\/:*?""<>|]+$"))
            {
                New.Background = Brushes.LightGreen;
                Apply.IsEnabled = true;
            }
            else
            {
                New.Background = Brushes.LightCoral;
                Apply.IsEnabled = false;
            }

            if (New.Text.Length == 0)
            {
                New.Background = Brushes.White;
                Apply.IsEnabled = true;
            }
        }
    }
}
