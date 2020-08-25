using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public static class FileLoader
    {
        private static Dictionary<string, string> textFileCache = new Dictionary<string, string>();

        public static void LoadAllComponentGroups()
        {
            if (Directory.Exists(ComponentGroup.ROOT_PATH))
            {
                var files = Directory.GetFiles(ComponentGroup.ROOT_PATH, $"*.{ComponentGroup.FILE_EXTENSION}", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    LoadTextFile(file);
                }
            }
        }

        /// <summary>
        /// Loads text file from storage.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true if success, false otherwise</returns>
        public static bool LoadTextFile(string path)
        {
            path = path.ToLower();

            bool result = true;
            try
            {
                string contents = File.ReadAllText(path);
                textFileCache[path] = contents;
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
        /// <param name="forceReload">If true, will reload the text file contents from storage even if it has already been loaded to the chace dictionary. Otherwise it will only load from storage if the path has not already been loaded into the chache dictionary.</param>
        /// <returns>true if success, false otherwise</returns>
        public static bool GetTextFileConents(string path, out string contents, bool forceReload = false)
        {
            path = path.ToLower();

            if (forceReload || !textFileCache.ContainsKey(path))
            {
                LoadTextFile(path);
            }

            if (textFileCache.ContainsKey(path))
            {
                contents = textFileCache[path];
                return true;
            }
            else
            {
                contents = null;
                return false;
            }
        }

        public static void SaveTextFile(string path, string contents)
        {
            path = path.ToLower();

            File.WriteAllText(path, contents);
            textFileCache[path] = contents;
        }
    }
}
