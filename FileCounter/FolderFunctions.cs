using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileCounter
{
    public static class FolderFunctions
    {
        public static event EventHandler RedrawTreeEvent;
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
        public static void MoveUp(object sender, RoutedEventArgs args)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is CustomFolder folder && folder.ParentFolder != null)
            {
                int index = folder.ParentFolder.Children.IndexOf(folder);
                folder.ParentFolder.Children.Remove(folder);
                folder.ParentFolder.Children.Insert(Math.Clamp(index - 1, 0, int.MaxValue), folder);
                folder.ParentFolder.NumberChildren();
                RedrawTreeEvent?.Invoke(folder,new());
            }
        }
    }
}
