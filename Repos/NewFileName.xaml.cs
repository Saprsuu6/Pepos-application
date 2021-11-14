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
    /// Interaction logic for NewFileName.xaml
    /// </summary>
    public partial class NewFileName : Window
    {
        public NewFileName()
        {
            InitializeComponent();
            New.Focus();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool SameName(FileInfo file)
        {
            for (int i = 0; i < (Owner as MainWindow).FilesName.Items.Count; i++)
            {
                if ((string)(Owner as MainWindow).FilesName.Items[i] == New.Text + file.Extension)
                    return true;
            }

            return false;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (New.Text.Length > 0)
            {
                foreach (FileInfo file in (Owner as MainWindow).WorkWithDirectory.FileInfos)
                {
                    if (file.Name == (string)(Owner as MainWindow).FilesName.SelectedItem)
                    {
                        if (!SameName(file))
                        {
                            File.Move(file.FullName, (Owner as MainWindow).WorkWithDirectory.DirectoryInfo.FullName + @"\" + New.Text + file.Extension);
                            file.Delete();
                        }
                        else
                        {
                            MessageBox.Show("Файл с таким названием уже существует", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                        }
                    }                        
                }

                Close();
            }
            else
            {
                MessageBox.Show("Введите навзание файла", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Error);
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
