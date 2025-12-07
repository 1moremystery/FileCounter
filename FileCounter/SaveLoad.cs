using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

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
                tvi.DataContext = customFolder;

                CheckBox box = new();
                box.Content = customFolder.Path;
                tvi.Header = box;

                //binding
                Binding binding = new(nameof(customFolder.DoCount));
                binding.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(box, CheckBox.IsCheckedProperty, binding);


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
                treeview.DataContext = item;
                CheckBox box = new();
                box.Content = item.Path;
                
                //binding
                Binding binding = new(nameof(item.DoCount));
                binding.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(box, CheckBox.IsCheckedProperty, binding);

                treeview.Header = box;
                AddChildrenToTree(item, treeview);
                tvi.Items.Add(treeview);
            }
        }

        public static void DoOutput(List<CustomFolder> list)
        {
            List<Tuple<string, int>> output = new();
            foreach (CustomFolder item in list)
            {
                //if count this one, don't count children
                //would duplicate numbers
                if (item.DoCount)
                {
                    output.Add(new(item.Path, item.Count));
                }
                else
                {
                    foreach (CustomFolder child in item.Children)
                    {
                        OutputOfChild(output, child);
                    }
                }
            }
            OutputToFile(output);
        }

        private static void OutputOfChild(List<Tuple<string, int>> outputList, CustomFolder child)
        {
            //if count this one, don't count children
            //would duplicate numbers
            if (child.DoCount)
            {
                outputList.Add(new(child.Path, child.Count));
            }
            else
            {
                foreach (CustomFolder nChild in child.Children) OutputOfChild(outputList, nChild);
            }
        }

        private static void OutputToFile(List<Tuple<string, int>> list)
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
