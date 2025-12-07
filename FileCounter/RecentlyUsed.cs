using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;

namespace FileCounter
{
    public static class RecentlyUsed
    {
        /// <summary>
        /// Gets recent stuff
        /// </summary>
        /// <param name="recentFiles">List of all recently opened files</param>
        /// <returns>Most recently opened file</returns>
        public static string GetRecents(out List<string> recentFiles)
        {
            recentFiles = new();
            if(!File.Exists("recent.xml")) return "";

            using (XmlReader reader = XmlReader.Create("recent.xml"))
            {
                while (reader.Read())
                {
                    if(reader.IsStartElement() && reader.Name == "file")
                    {
                        recentFiles.Add(reader.ReadString());
                    }
                }
            }
            if (recentFiles.Count == 0) return "";
            else return recentFiles[0];
        }

        public static void WriteRecent(IEnumerable<string> recentFiles)
        {
            using(XmlWriter writer = XmlWriter.Create("recent.xml", new() { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("root");
                foreach (string file in recentFiles)
                {
                    writer.WriteStartElement("file");
                    writer.WriteString(file);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

            }
        }

        public static void AddToRecent(List<string> recentFiles, string nPath)
        {
            if (recentFiles.Remove(nPath))
            {
                recentFiles.Insert(0,nPath);
            }
            else
            {
                recentFiles.Add(nPath);
            }
            WriteRecent(recentFiles);
        }
    }
}
