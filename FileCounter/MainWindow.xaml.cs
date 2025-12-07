using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<CustomFolder> topLevelFolders = new();
        List<string> recentFiles;

        public MainWindow()
        {
            InitializeComponent();
            string mostRecent = RecentlyUsed.GetRecents(out  recentFiles);
            if (!string.IsNullOrEmpty(mostRecent))
            {
                topLevelFolders = SaveLoad.LoadFromFile(mostRecent);
                SaveLoad.SetupTree(topLevelFolders, FolderTree);
                RecentToMenuItem(recentFiles);
            }
        }

        public void RecentToMenuItem(List<string> recentFiles)
        {
            RecentFilesMenuItem.Items.Clear();
            if(recentFiles.Count > 0 )
            {
                RecentFilesMenuItem.IsEnabled = true;
                MenuItem remove = new();
                remove.Header = "Remove Recents";
                remove.Click += RemoveRecentClick;
                RecentFilesMenuItem.Items.Add(remove);
            }
            foreach (string file in recentFiles)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = file;
                menuItem.Click += RecentFile_Click;
                RecentFilesMenuItem.Items.Add(menuItem);
            }
        }

        private void RecentFile_Click(object sender, RoutedEventArgs e)
        {
            if(sender is MenuItem menuItem && menuItem.Header is string s)
            { 
                topLevelFolders = SaveLoad.LoadFromFile(s);
                SaveLoad.SetupTree(topLevelFolders, FolderTree);
                RecentlyUsed.AddToRecent(recentFiles,s);
                RecentToMenuItem(recentFiles);
            }
        }

        private void RemoveRecentClick(object sender, RoutedEventArgs e)
        {
            recentFiles.Clear();
            RecentFilesMenuItem.Items.Clear();
            RecentFilesMenuItem.IsEnabled = false;
            RecentlyUsed.WriteRecent(recentFiles);
            RecentToMenuItem(recentFiles);
        }

        /// <summary>
        /// Handles when load folder button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                topLevelFolders = SaveLoad.LoadFolder(dialog.FolderName, FolderTree);
                NumberTopFolders();
            }
        }

        /// <summary>
        /// Handles when print output is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintOutput_Click(object sender, RoutedEventArgs e)
        {
            SaveLoad.DoOutput(topLevelFolders);
        }

        /// <summary>
        /// Handles when save button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveStructure_Click(object sender, RoutedEventArgs e)
        {
            if (topLevelFolders.Count == 0)
            {
                MessageBox.Show("Add some folders first");
                return;
            }
            SaveFileDialog dialog = new() { ValidateNames = true, Filter = "File Structure File (*.fsf)|*.fsf" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                try
                {
                    SaveLoad.SaveFoldersToFile(topLevelFolders, dialog.FileName);
                    RecentlyUsed.AddToRecent(recentFiles, dialog.FileName);
                    RecentToMenuItem(recentFiles);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Something went wrong:\n{ex.Message}", "Something Went Wrong");
                }
            }
        }

        private void LoadStructure_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new() { ValidateNames = true, Filter = "File Structure File (*.fsf)|*.fsf" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                topLevelFolders = SaveLoad.LoadFromFile(dialog.FileName);
                topLevelFolders.Sort();
                FolderTree.Items.Clear();
                SaveLoad.SetupTree(topLevelFolders,FolderTree);
                RecentlyUsed.AddToRecent(recentFiles,dialog.FileName);
                RecentToMenuItem(recentFiles);
            }
        }

        private void NumberTopFolders()
        {
            for (int i = 0; i < topLevelFolders.Count; i++)
            {
                topLevelFolders[i].Order = i;
            }
        }
    }
}