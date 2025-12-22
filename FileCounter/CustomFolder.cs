using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileCounter
{
    public class CustomFolder : IComparable<CustomFolder>
    {
        public string Path { get; set; }
        public List<CustomFolder> Children = new();
        public int Order { get; set; }

        /// <summary>
        /// Parent folder of this folder if it exists
        /// </summary>
        public CustomFolder? ParentFolder { get; set; }
        /// <summary>
        /// If this folder is a top level folder this is where the list of all will go
        /// </summary>
        public List<CustomFolder>? TopLevelFoldersList { get; set;}

        public int Count
        {
            get
            {
                int count = Directory.GetFiles(Path).Length;
                foreach (var child in Children) count += child.Count;
                return count;
            }
        }
        /// <summary>
        /// Whether to count files in this folder 
        /// </summary>
        public bool DoCount { get; set; }

        public CustomFolder(string path, CustomFolder? parentFolder = null)
        {
            Path = path;
            ParentFolder = parentFolder;
            SetupChildren();
            NumberChildren();
        }

        public CustomFolder(string path, List<CustomFolder> children, bool doCount, int order, CustomFolder? parentFolder = null)
        {
            if (!Directory.Exists(path)) throw new FileNotFoundException("This Folder wasn't found!");
            Path = path;
            Children = children;
            DoCount = doCount;
            Order = order;
            Children.Sort();
            ParentFolder = parentFolder;
        }

        /// <summary>
        /// Sets each child to what number in the order it is
        /// </summary>
        public void NumberChildren(bool recurse = false)
        {
            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].Order = i;
                if(recurse) Children[i].NumberChildren(true);
            }
        }

        void SetupChildren()
        {
            foreach (string s in Directory.GetDirectories(Path))
            {
                Children.Add(new(s,this));
            }
        }

        public override string ToString()
        {
            return $"{Order} {Path}";
        }

        public int CompareTo(CustomFolder? other)
        {
            if (other == null) return 1;
            else return Order.CompareTo(other.Order);
        }
    }
}
