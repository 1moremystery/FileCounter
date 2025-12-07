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

        public MainWindow()
        {
            InitializeComponent();
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
                FolderTree.Items.Clear();
                SaveLoad.SetupTree(topLevelFolders,FolderTree);
            }
        }
    }
}