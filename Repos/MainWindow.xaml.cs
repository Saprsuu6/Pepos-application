using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Repos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WorkWithDirectory workWithDirectory = new WorkWithDirectory();

        internal WorkWithDirectory WorkWithDirectory { get => workWithDirectory; }
        public ListBox FilesName { get => Files; }

        public MainWindow()
        {
            InitializeComponent();
            RepoName.Focus();

            if (!Directory.Exists(@"C:\Repos"))
            {
                Directory.CreateDirectory(@"C:\Repos");
                DirectoryInfo info = new DirectoryInfo(@"C:\Repos");
                info.Attributes = FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.Encrypted;
            }

            ReloadDirectories(@"C:\Repos");

            if (Repos.Items.Count > 0)
            {
                Repos.IsEnabled = true;
                Files.IsEnabled = true;
            }
        }

        private void ReloadDirectories(string path)
        {
            WorkWithDirectory.DirectoryInfo = new DirectoryInfo(path);
            DirectoryInfo[] directoryInfos = WorkWithDirectory.DirectoryInfo.GetDirectories();

            Repos.Items.Clear();
            Files.Items.Clear();
            Extention.Items.Clear();

            foreach (DirectoryInfo file in directoryInfos)
                Repos.Items.Add(file.Name);

            if (Repos.Items.Count > 0)
                Repos.SelectedItem = Repos.Items[0];
            else
            {
                Repos.IsEnabled = false;
                Files.IsEnabled = false;
                RemoveFile.IsEnabled = false;
                RenameFile.IsEnabled = false;
                RemoveFolder.IsEnabled = false;
            }
        }

        private void ReloadFiles(string path, string extention)
        {
            WorkWithDirectory.DirectoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfos = WorkWithDirectory.DirectoryInfo.GetFiles();

            Files.Items.Clear();

            foreach (FileInfo file in fileInfos)
            {
                if (Regex.IsMatch(file.Name, extention))
                    Files.Items.Add(file.Name);
            }

            RemoveFile.IsEnabled = false;
            RenameFile.IsEnabled = false;
        }

        private void ReloadExtentions(string path)
        {
            WorkWithDirectory.DirectoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfos = WorkWithDirectory.DirectoryInfo.GetFiles();

            Extention.Items.Clear();

            foreach (FileInfo file in fileInfos)
            {
                if (!Extention.Items.Contains("*" + file.Extension))
                    Extention.Items.Add("*" + file.Extension);
            }

            if (fileInfos.Length > 0)
            {
                Extention.Items.Insert(0, "*.*");
                Extention.SelectedItem = Extention.Items[0].ToString();
            }
            else
            {
                Files.Items.Clear();
                RemoveFile.IsEnabled = false;
                RenameFile.IsEnabled = false;
            }
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            if (RepoName.Text.Length > 0)
            {
                Repos.IsEnabled = true;
                Files.IsEnabled = true;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == true)
                {
                    string[] filesPath = openFileDialog.FileNames;

                    List<FileInfo> filesInfo = new List<FileInfo>();
                    foreach (string file in filesPath)
                    {
                        filesInfo.Add(new FileInfo(file));

                        Directory.CreateDirectory(@"C:\Repos\" + RepoName.Text);

                        if (File.Exists(@"C:\Repos\" + RepoName.Text + @"\" + filesInfo[filesInfo.Count - 1].Name))
                        {
                            MessageBoxResult result = MessageBox.Show("Файл с именем " + filesInfo[filesInfo.Count - 1].Name + " уже сущществует.",
                                "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                            File.Move(file, @"C:\Repos\" + RepoName.Text + @"\" + filesInfo[filesInfo.Count - 1].Name);
                    }

                    if (!Repos.Items.Contains(RepoName.Text))
                    {
                        Repos.Items.Add(RepoName.Text);
                        Repos.SelectedItem = Repos.Items[0];
                    }

                    ReloadExtentions(@"C:\Repos\" + RepoName.Text);

                    RepoName.Text = "";
                }
            }
            else 
            { 
                MessageBox.Show("Введите навзание хранилища", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Error);
                RepoName.Focus();
            }
        }

        private void RepoName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.IsMatch(RepoName.Text, @"^[^\/:*?""<>|]+$"))
            {
                RepoName.Background = Brushes.LightGreen;
                NewFile.IsEnabled = true;
            }
            else
            {
                RepoName.Background = Brushes.LightCoral;
                NewFile.IsEnabled = false;
            }

            if (RepoName.Text.Length == 0)
            {
                RepoName.Background = Brushes.White;
                NewFile.IsEnabled = true;
            }
        }

        private void Repos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveFolder.IsEnabled = true;
            ReloadExtentions(@"C:\Repos\" + Repos.SelectedItem);

            WorkWithDirectory.DirectoryInfo = new DirectoryInfo(@"C:\Repos\" + Repos.SelectedItem);
            WorkWithDirectory.FileInfos = WorkWithDirectory.DirectoryInfo.GetFiles();

            if (Repos.SelectedItem == null)
                RemoveFolder.IsEnabled = false;
        }

        private void Files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveFile.IsEnabled = true;
            RenameFile.IsEnabled = true;
        }

        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Вы точно хотите удалить это хранилище?", "Сообщене", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dialogResult == MessageBoxResult.Yes) {
                Directory.Delete(@"C:\Repos\" + Repos.SelectedItem, true);
                ReloadDirectories(@"C:\Repos");
            }
        }

        private void RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Вы точно хотите удалить этот файл?", "Сообщене", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dialogResult == MessageBoxResult.Yes)
            {
                IList files = Files.SelectedItems;

                foreach (var file in files)
                    File.Delete(@"C:\Repos\" + Repos.SelectedItem + @"\" + file);

                ReloadExtentions(@"C:\Repos\" + Repos.SelectedItem);
                WorkWithDirectory.FileInfos = WorkWithDirectory.DirectoryInfo.GetFiles();
            }
        }

        private void SeacrchConcreteFile(string name)
        {
            Files.Items.Clear();

            foreach (FileInfo file in WorkWithDirectory.FileInfos)
            {
                if (file.Name.ToLower().Contains(name.ToLower()))
                    Files.Items.Add(file.Name);
            }
        }

        private void FileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            SeacrchConcreteFile(FileName.Text);
            FileName.IsEnabled = true;
        }

        private void Extention_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string extention = (string)Extention.SelectedItem;

            if (extention != null)
            {
                if (extention[2] == '*')
                    extention = @"^[^\/:*?""<>|]+\.\w+$";
                else
                    extention = @"^[^\/:*?""<>|]+\." + extention.Substring(2) + "$";

                ReloadFiles(@"C:\Repos\" + Repos.SelectedItem, extention);
            }
        }

        private void Files_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Files.SelectedItem != null)
            {
                string commandText = WorkWithDirectory.DirectoryInfo.FullName + @"\" + Files.SelectedItem;
                Process proc = new Process();
                proc.StartInfo.FileName = commandText;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
        }

        private void RenameRepo_Click(object sender, RoutedEventArgs e)
        {
            Effect = new BlurEffect();

            NewFolderName newName = new NewFolderName();
            newName.Owner = this;
            newName.ShowDialog();
            ReloadDirectories(@"C:\Repos");

            Effect = null;
        }

        private void RenameFile_Click(object sender, RoutedEventArgs e)
        {
            Effect = new BlurEffect();

            NewFileName newName = new NewFileName();
            newName.Owner = this;
            newName.ShowDialog();
            ReloadDirectories(@"C:\Repos");

            Effect = null;
        }
    }
}
