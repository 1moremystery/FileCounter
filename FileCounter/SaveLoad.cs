using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace FileCounter
{
    public static class SaveLoad
    {
        /// <summary>
        /// Loads folders and then adds to the tree
        /// </summary>
        /// <param name="path">Path to start on</param>
        /// <param name="tree">Tree to add onto</param>
        /// <returns>List of top level folders</returns>
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

        /// <summary>
        /// Saves all folders count to a output txt
        /// </summary>
        /// <param name="list">Top level folders list</param>
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

        /// <summary>
        /// Adds all children folder's count to output list
        /// </summary>
        /// <param name="outputList">List that goes to output file</param>
        /// <param name="child">Child folder to check</param>
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

        /// <summary>
        /// Writes the list to a file
        /// </summary>
        /// <param name="list">List of path and count</param>
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


        public static void SaveFoldersToFile(IEnumerable<CustomFolder> folders, string path)
        {
            using (XmlWriter output = XmlWriter.Create(path, new() { Indent = true, ConformanceLevel = ConformanceLevel.Auto }))
            {
                output.WriteStartDocument();
                output.WriteStartElement("FolderStructure");
                foreach (CustomFolder f in folders)
                {
                    WriteFolder(f, output);
                }
                output.WriteEndElement();
            }
        }

        private static void WriteFolder(CustomFolder folder, XmlWriter writer)
        {
            writer.WriteStartElement("folder");
            writer.WriteAttributeString("doCount", folder.DoCount.ToString());
            writer.WriteAttributeString("path", folder.ToString());
            //writer.WriteValue(folder.DoCount);
            foreach(CustomFolder child in folder.Children)
            {
                WriteFolder(child, writer);
            }
            writer.WriteEndElement();
        }
    }
}
