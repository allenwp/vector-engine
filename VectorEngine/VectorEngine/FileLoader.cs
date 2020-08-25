using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VectorEngine
{
    public static class FileLoader
    {
        /// <summary>
        /// Where the key is the relative path to the file. (Relative to the AssetsPath)
        /// Keys are always lower case versions.
        /// </summary>
        private static Dictionary<string, string> textFileCache = new Dictionary<string, string>();
        private static string assetsPath;

        public static void Init(string assetsPath)
        {
            FileLoader.assetsPath = assetsPath;
        }

        public static void LoadAllComponentGroups()
        {
            foreach (var file in GetAllComponentGroupPaths())
            {
                LoadTextFile(file);
            }
        }

        /// <returns>An array of paths that are </returns>
        public static string[] GetAllComponentGroupPaths()
        {
            string componentGroupDirectory = FullPath(ComponentGroup.ROOT_PATH);
            if (Directory.Exists(componentGroupDirectory))
            {
                string[] fullPaths = Directory.GetFiles(componentGroupDirectory, $"*.{ComponentGroup.FILE_EXTENSION}", SearchOption.AllDirectories);
                string[] relativeFiles = new string[fullPaths.Length];
                for (int i = 0; i < fullPaths.Length; i++)
                {
                    string filename = Regex.Match(fullPaths[i], $"{ComponentGroup.ROOT_PATH}.*").Value;
                    relativeFiles[i] = filename;
                }
                return relativeFiles;
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Loads text file from storage.
        /// </summary>
        /// <param name="relativePath">Path relative to the Assets Path</param>
        /// <returns>true if success, false otherwise</returns>
        public static bool LoadTextFile(string relativePath)
        {
            string fullPath = FullPath(relativePath);

            bool result = true;
            try
            {
                string contents = File.ReadAllText(fullPath);
                textFileCache[relativePath.ToLower()] = contents;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Get the contents of a text file from the cache dictionary or from storage if it hasn't been cached already.
        /// </summary>
        /// <param name="relativePath">Path relative to the Assets Path</param>
        /// <param name="forceReload">If true, will reload the text file contents from storage even if it has already been loaded to the chace dictionary. Otherwise it will only load from storage if the path has not already been loaded into the chache dictionary.</param>
        /// <returns>true if success, false otherwise</returns>
        public static bool GetTextFileConents(string relativePath, out string contents, bool forceReload = false)
        {
            if (forceReload || !textFileCache.ContainsKey(relativePath.ToLower()))
            {
                LoadTextFile(relativePath);
            }

            if (textFileCache.ContainsKey(relativePath.ToLower()))
            {
                contents = textFileCache[relativePath.ToLower()];
                return true;
            }
            else
            {
                contents = null;
                return false;
            }
        }

        /// <param name="relativePath">Path relative to the Assets Path</param>
        public static void SaveTextFile(string relativePath, string contents)
        {
            string fullPath = FullPath(relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            File.WriteAllText(fullPath, contents);
            textFileCache[relativePath.ToLower()] = contents;
        }

        /// <summary>
        /// Turns a path that is relative to the Assets Path into a full system-usable path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FullPath(string path)
        {
            return Path.GetFullPath(Path.Combine(assetsPath, path));
        }
    }
}
