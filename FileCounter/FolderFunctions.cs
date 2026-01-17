using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FileCounter
{
    public static class FolderFunctions
    {
        /// <summary>
        /// A redraw of the tree is needed, bool is whether the top level folders need to be reordered
        /// </summary>
        public static event EventHandler<bool> RedrawTreeEvent;
        /// <summary>
        /// Sorts the folders and folder's children by path name
        /// </summary>
        /// <param name="folders">top level folders</param>
        /// <returns>Sorted list</returns>
        public static List<CustomFolder> SortFolderAndChildrenByName(List<CustomFolder> folders)
        {
            List<CustomFolder> sorted = folders.OrderBy(o => o.Path).ToList();
            foreach (CustomFolder folder in sorted)
            {
                folder.Children = SortFolderAndChildrenByName(folder.Children);
            }
            ResetOrder(sorted);
            return sorted;
        }

        /// <summary>
        /// Sets the order of folder to what they are
        /// </summary>
        /// <param name="folders">List of folder</param>
        public static void ResetOrder(List<CustomFolder> folders)
        {
            for (int i = 0; i < folders.Count; i++)
            {
                folders[i].Order = i;
                folders[i].NumberChildren();
            }
        }

        /// <summary>
        /// When a move up button is clicked
        /// </summary>
        public static void MoveUp_Click(object sender, RoutedEventArgs args)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is CustomFolder folder)
            {
                if(folder.ParentFolder != null) //this is a child folder
                {
                    int index = folder.ParentFolder.Children.IndexOf(folder);
                    folder.ParentFolder.Children.Remove(folder);
                    folder.ParentFolder.Children.Insert(Math.Clamp(index - 1, 0, int.MaxValue), folder);
                    folder.ParentFolder.NumberChildren();
                    RedrawTreeEvent?.Invoke(folder, false);
                }
                else if(folder.TopLevelFoldersList != null) //this is a topLevel folder
                {
                    int index = folder.TopLevelFoldersList.IndexOf(folder);
                    folder.TopLevelFoldersList.Remove(folder);
                    folder.TopLevelFoldersList.Insert(Math.Clamp(index - 1, 0, int.MaxValue), folder);
                    RedrawTreeEvent?.Invoke(folder, true);
                }
            }
        }

        public static void MoveDown_Click(object sender, RoutedEventArgs args)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is CustomFolder folder)
            {
                if (folder.ParentFolder != null)
                {
                    int index = folder.ParentFolder.Children.IndexOf(folder);
                    index = Math.Clamp(index+1, 0, folder.ParentFolder.Count-1);
                    folder.ParentFolder.Children.Remove(folder);
                    folder.ParentFolder.Children.Insert(index, folder);
                    folder.ParentFolder.NumberChildren();
                    RedrawTreeEvent?.Invoke(folder, false);
                }
                else if (folder.TopLevelFoldersList != null)
                {
                    int index = folder.TopLevelFoldersList.IndexOf(folder);
                    index = Math.Clamp(index+1,0, folder.TopLevelFoldersList.Count-1);
                    folder.TopLevelFoldersList.Remove(folder);
                    folder.TopLevelFoldersList.Insert(index, folder);
                    RedrawTreeEvent?.Invoke(folder, true);
                }
            }
        }

        public static void CheckAllChildren_Click(object sender, RoutedEventArgs args)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is CustomFolder folder)
            {
                foreach(CustomFolder child in folder.Children)
                {
                    child.DoCount = true;
                }
            }
        }

        public static void UncheckAllChildren_Click(object sender, RoutedEventArgs args)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is CustomFolder folder)
            {
                foreach (CustomFolder child in folder.Children)
                {
                    child.DoCount = false;
                }
            }
        }

        public static void ResetChildrenOrder_Click(object sender, RoutedEventArgs args)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is CustomFolder folder)
            {
                folder.ResetChildrenOrder();
                RedrawTreeEvent?.Invoke(null, false);
            }
        }

        public static void CheckForNewChildren_click(object sender, RoutedEventArgs args)
        {
            if (sender is MenuItem menuitem && menuitem.DataContext is CustomFolder folder)
            {
                List<string> newshit = new();
                CheckNewChildren(folder,newshit);
                if(newshit.Count > 0)
                {
                    folder.NumberChildren(true);
                    StringBuilder sb = new("Found new Folders:\n");
                    foreach(string s in newshit) sb.AppendLine(s);
                    MessageBox.Show(sb.ToString());
                    //TODO: print new folder paths to file
                    RedrawTreeEvent?.Invoke(null, false);
                }
            }
        }

        /// <summary>
        /// Checks the given folder for new children
        /// </summary>
        /// <param name="folder">Folder to check</param>
        /// <param name="anythingNew">Where new folder children are found</param>
        private static void CheckNewChildren(CustomFolder folder, List<string> anythingNew)
        {
            List<string> existingChildren = new();
            foreach (CustomFolder f in folder.Children)
            {
                CheckNewChildren(f, anythingNew);
                existingChildren.Add(f.Path);
            }
            IEnumerable<string> newPaths = Directory.GetDirectories(folder.Path).Where(np => !existingChildren.Contains(np));
            
            foreach(string path in newPaths)
            {
                anythingNew.Add(path);
                CustomFolder newFolder = new(path, folder);
                folder.Children.Add(newFolder);

                //gets any children of this new folder and adds to list
                List<CustomFolder> cF = new();
                RecursiveAddChildrenOfFolderToList(newFolder, cF);
                anythingNew.AddRange(cF.Select(f => f.Path));
            }
            return;
        }

        public static void RecursiveAddChildrenOfFolderToList(CustomFolder folder, List<CustomFolder> list)
        {
            foreach(CustomFolder child in folder.Children)
            {
                list.Add(child);
                RecursiveAddChildrenOfFolderToList(child, list);
            }
        }
    }
}
