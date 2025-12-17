using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCounter
{
    public static class FolderFunctions
    {
        /// <summary>
        /// Sorts the folders and folder's children by path name
        /// </summary>
        /// <param name="folders">top level folders</param>
        /// <returns>Sorted list</returns>
        public static List<CustomFolder> SortFolderAndChildrenByName(List<CustomFolder> folders)
        {
            List<CustomFolder> sorted = folders.OrderBy(o => o.Path).ToList();
            //int index = 0;
            foreach(CustomFolder folder in sorted)
            {
                //folder.Order = index;
                folder.Children = SortFolderAndChildrenByName(folder.Children);
                //index++;
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
            for(int i = 0; i < folders.Count; i++)
            {
                folders[i].Order = i;
                folders[i].NumberChildren();
            }
        }
    }
}
