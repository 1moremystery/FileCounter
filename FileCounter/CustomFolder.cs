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
                int count = 0;
                if (CountChildren)
                {
                    foreach(CustomFolder child in Children)
                    {
                        count += child.Count;
                    }
                }
                else
                {
                    count = Directory.GetFiles(Path).Count();
                }
                return count;
            }
        }
        public bool CountChildren { get; set; }

        public CustomFolder(string path, bool countChildren = false)
        {
            Path = path;
            SetupChildren();
            CountChildren = countChildren;
        }

        void SetupChildren()
        {
            foreach(string s in Directory.GetDirectories(Path))
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
