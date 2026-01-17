using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Xml;

namespace FileCounter
{
    public static class SaveLoad
    {
        static ContextMenu globalContextMenu = new();
        static SaveLoad()
        {
            //Context menu
            MenuItem resetChildrenOrder = new() { Header= "Reset Children Order"};
            resetChildrenOrder.Click += FolderFunctions.ResetChildrenOrder_Click;

            MenuItem moveUp = new MenuItem() { Header = "Move Up" };
            moveUp.Click += FolderFunctions.MoveUp_Click;
            MenuItem moveDown = new MenuItem() { Header = "Move Down" };
            moveDown.Click += FolderFunctions.MoveDown_Click;

            MenuItem CheckChildren = new() { Header = "Check All Children" };
            CheckChildren.Click += FolderFunctions.CheckAllChildren_Click;
            
            MenuItem UnCheckChildren = new() { Header = "Uncheck All Children" };
            UnCheckChildren.Click += FolderFunctions.UncheckAllChildren_Click;

            MenuItem CheckForNewChilren = new() { Header = "Check for new children" };
            CheckForNewChilren.Click += FolderFunctions.CheckForNewChildren_click;

            globalContextMenu.Items.Add(resetChildrenOrder);
            globalContextMenu.Items.Add(moveUp);
            globalContextMenu.Items.Add(moveDown);
            globalContextMenu.Items.Add(new Separator());
            globalContextMenu.Items.Add(CheckChildren);
            globalContextMenu.Items.Add(UnCheckChildren);
            globalContextMenu.Items.Add(CheckForNewChilren);
        }

        /// <summary>
        /// Creates a new treeViewItem with context menu and dataContext
        /// </summary>
        /// <param name="folder">folder to be dataContext</param>
        /// <returns>TreeView item</returns>
        private static TreeViewItem CreateNewTreeViewItem(CustomFolder folder)
        {
            TreeViewItem treeView = new();
            treeView.DataContext = folder;
            treeView.ContextMenu = globalContextMenu;
            treeView.Header = new FolderControl();
            return treeView;
        }

        /// <summary>
        /// Loads customfolders from given folder path
        /// </summary>
        /// <param name="path">Path to start on</param>
        /// <returns>List of top level folders</returns>
        public static List<CustomFolder> LoadFolder(string path)
        {
            List<CustomFolder> result = new();
            foreach (string fPath in Directory.GetDirectories(path))
            {
                CustomFolder customFolder = new(fPath);
                result.Add(customFolder);
            }
            return result;
        }

        /// <summary>
        /// Adds all children of a folder to it's tree item
        /// </summary>
        /// <param name="customFolder">Folder to get children of</param>
        /// <param name="tvi">Tree item to add children too</param>
        private static void AddChildrenToTree(CustomFolder customFolder, TreeViewItem tvi)
        {
            foreach (CustomFolder childFolder in customFolder.Children)
            {
                TreeViewItem treeView = CreateNewTreeViewItem(childFolder);

                //recursive call
                AddChildrenToTree(childFolder, treeView);
                
                tvi.Items.Add(treeView);
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
                    writer.WriteLine($"{item.Item2}");
                }
            }
        }

        /// <summary>
        /// Save folders to a file
        /// </summary>
        /// <param name="folders">List of topLevel folders</param>
        /// <param name="path">path to save to</param>
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

        /// <summary>
        /// Writes the folder to xmlWriter
        /// </summary>
        /// <param name="folder">Folder to write</param>
        /// <param name="writer">Xml writer</param>
        private static void WriteFolder(CustomFolder folder, XmlWriter writer)
        {
            writer.WriteStartElement("folder");
            writer.WriteAttributeString("doCount", folder.DoCount.ToString());
            writer.WriteAttributeString("path", folder.Path);
            writer.WriteAttributeString("order", folder.Order.ToString());
            foreach (CustomFolder child in folder.Children)
            {
                WriteFolder(child, writer);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Sets up the tree
        /// </summary>
        /// <param name="folders">List of top folders to add</param>
        /// <param name="tree">TreeView to add to</param>
        public static void SetupTree(IEnumerable<CustomFolder> folders, TreeView tree)
        {
            tree.Items.Clear();
            foreach (CustomFolder customFolder in folders)
            {
                TreeViewItem treeItem = CreateNewTreeViewItem(customFolder);

                AddChildrenToTree(customFolder, treeItem);
                tree.Items.Add(treeItem);
            }
        }

        /// <summary>
        /// Load File from file
        /// </summary>
        /// <param name="path">Path of file</param>
        /// <returns>List of folders</returns>
        public static List<CustomFolder> LoadFromFile(string path)
        {
            List<CustomFolder> topLevelFolders = new List<CustomFolder>();
            using (XmlReader reader = XmlReader.Create(path))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "folder")
                        {
                            string? count = reader.GetAttribute("doCount");
                            string? fpath = reader.GetAttribute("path");
                            string? sOrder = reader.GetAttribute("order");
                            int order = 0;
                            bool doCount = false;
                            if (count != null) doCount = bool.Parse(count);
                            if (sOrder != null) order = int.Parse(sOrder);
                            if (fpath == null) throw new NullReferenceException("Malformed File Structure File, (missing path attribute)");

                            try
                            {
                                //toplevel folder so parent is null
                                CustomFolder folda = new CustomFolder(fpath, new(), doCount, order);
                                //but add the folder to the top level list
                                folda.TopLevelFoldersList = topLevelFolders;
                                if (!reader.IsEmptyElement)
                                {
                                    folda.Children = LoadChildFolders(reader, folda);
                                }
                                topLevelFolders.Add(folda);
                            }
                            catch
                            {
                                MessageBox.Show($"Failed to load: {fpath}");
                            }

                        }
                    }
                }
            }

            return topLevelFolders;
        }

        /// <summary>
        /// Loads the children folders from the xml reader
        /// </summary>
        /// <param name="reader">XML reader</param>
        /// <returns>List of children folders</returns>
        private static List<CustomFolder> LoadChildFolders(XmlReader reader, CustomFolder parent)
        {
            List<CustomFolder> childrenList = new();
            while (reader.Read() && reader.IsStartElement())
            {
                string? count = reader.GetAttribute("doCount");
                string? fpath = reader.GetAttribute("path");
                string? sOrder = reader.GetAttribute("order");
                int order = 0;
                bool doCount = false;
                if (count != null) doCount = bool.Parse(count);
                if (sOrder != null) order = int.Parse(sOrder);
                if (fpath == null) throw new NullReferenceException("Malformed File Structure File, (missing path attribute)");
                try
                {
                    CustomFolder folder = new(fpath, new(), doCount, order, parent);
                    if (!reader.IsEmptyElement)
                    {
                        folder.Children = LoadChildFolders(reader, folder);
                    }
                    childrenList.Add(folder);

                }
                catch
                {
                    MessageBox.Show($"Failed to load: {fpath}");
                }
            }
            return childrenList;
        }
    }
}
