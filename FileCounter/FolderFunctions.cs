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
            return sorted;
        }

    }
}
