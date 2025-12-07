using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileCounter
{
    public class CustomFolder
    {
        public string Path { get; set; }
        public List<CustomFolder> Children = new();
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
        }

        public CustomFolder(string path, List<CustomFolder> children, bool doCount)
        {
            Path = path;
            Children = children;
            DoCount = doCount;
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
            return Path;
        }
    }
}
