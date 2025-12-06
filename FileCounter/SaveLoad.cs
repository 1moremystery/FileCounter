using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FileCounter
{
    public static class SaveLoad
    {
        public static void LoadFolder(string path, TreeView tree)
        {
            tree.Items.Clear();
            foreach (string fPath in Directory.GetDirectories(path))
            {
                CustomFolder customFolder = new(fPath);
                TreeViewItem tvi = new();
                tvi.Header = customFolder.Path;
                tvi.DataContext = customFolder;
                AddChildrenToTree(customFolder, tvi);
                tree.Items.Add(tvi);
            }
        }

        private static void AddChildrenToTree(CustomFolder customFolder, TreeViewItem tvi)
        {
            foreach (CustomFolder item in customFolder.Children)
            {
                TreeViewItem treeview = new();
                treeview.Header = item.Path;
                treeview.DataContext = item;
                AddChildrenToTree(item, treeview);
                tvi.Items.Add(treeview);
            }
        }
    }
}
