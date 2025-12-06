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
        public static List<CustomFolder> LoadFolder(string path, TreeView tree)
        {
            tree.Items.Clear();
            List<CustomFolder> result = new();
            foreach (string fPath in Directory.GetDirectories(path))
            {
                CustomFolder customFolder = new(fPath);
                result.Add(customFolder);
                TreeViewItem tvi = new();
                tvi.Header = customFolder.Path;
                tvi.DataContext = customFolder;
                AddChildrenToTree(customFolder, tvi);
                tree.Items.Add(tvi);
            }
            return result;
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

        public static void DoOutput(List<CustomFolder> list)
        {
            List<Tuple<string,int>> output = new();
            foreach (CustomFolder item in list)
            {
                output.Add(new(item.Path, item.Count));
            }
            OutputToFile(output);
        }

        private static void OutputToFile(List<Tuple<string,int>> list)
        {
            using (StreamWriter writer = new("output.txt"))
            {
                foreach (var item in list)
                {
                    writer.WriteLine($"{item.Item1} {item.Item2}");
                }
            }
            using (StreamWriter writer = new("outputRaw.txt"))
            {
                foreach (var item in list)
                {
                    writer.WriteLine($"{item.Item1} {item.Item2}");
                }
            }
        }
    }
}
