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
        
        private void LoadFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                topLevelFolders = SaveLoad.LoadFolder(dialog.FolderName, FolderTree);
            }
        }

        private void PrintOutput_Click(object sender, RoutedEventArgs e)
        {
            SaveLoad.DoOutput(topLevelFolders);
        }
    }
}