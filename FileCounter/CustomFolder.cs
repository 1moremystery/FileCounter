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

        public int Count
        {
            get
            {
                int count = Directory.GetFiles(Path).Count();
                foreach (var child in Children) count += child.Count;
                return count;
            }
        }
        /// <summary>
        /// Whether to count files in this folder 
        /// </summary>
        public bool DoCount { get; set; }

        public CustomFolder(string path, bool doCount = false)
        {
            Path = path;
            DoCount = doCount;
            SetupChildren();
            NumberChildren();
        }

        public CustomFolder(string path, List<CustomFolder> children, bool doCount, int order)
        {
            if (!Directory.Exists(path)) throw new FileNotFoundException("This Folder wasn't found!");
            Path = path;
            Children = children;
            DoCount = doCount;
            Order = order;
            Children.Sort();
        }

        /// <summary>
        /// Sets each child to what number in the order it is
        /// </summary>
        public void NumberChildren()
        {
            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].Order = i;
                Children[i].NumberChildren();
            }
        }

        void SetupChildren()
        {
            foreach (string s in Directory.GetDirectories(Path))
            {
                Children.Add(new(s));
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
