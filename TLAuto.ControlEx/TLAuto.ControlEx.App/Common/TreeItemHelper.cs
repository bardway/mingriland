// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.IO;

using TLAuto.ControlEx.App.Models;
using TLAuto.ControlEx.App.Models.TreeModels;
#endregion

namespace TLAuto.ControlEx.App.Common
{
    public static class TreeItemHelper
    {
        public static string GetDirPath(string fullPath)
        {
            return Path.HasExtension(fullPath) ? Path.GetDirectoryName(fullPath) : fullPath;
        }

        public static string GetCreateFolderName(string templateName, string fullPath, int templateNameIncrease = 1)
        {
            var fullName = templateName + templateNameIncrease;
            var dirInfo = new DirectoryInfo(Path.Combine(fullPath, fullName));
            if (dirInfo.Exists)
            {
                templateNameIncrease++;
                return GetCreateFolderName(templateName, fullPath, templateNameIncrease);
            }
            return fullName;
        }

        public static string GetCreateControllerName(string templateName, string extensionName, string fullPath, int templateNameIncrease = 1)
        {
            var fullName = templateName + templateNameIncrease + "." + extensionName;
            var fileInfo = new FileInfo(Path.Combine(fullPath, fullName));
            if (fileInfo.Exists)
            {
                templateNameIncrease++;
                return GetCreateControllerName(templateName, extensionName, fullPath, templateNameIncrease);
            }
            return fullName;
        }

        public static void LoadProjectItems(TreeItemBase treeItem)
        {
            var dirPath = GetDirPath(treeItem.FullPath);
            var tlcFiles = Directory.EnumerateFiles(dirPath, "*" + ControllerInfo.EXTENSION, SearchOption.TopDirectoryOnly);
            foreach (var filePath in tlcFiles)
            {
                treeItem.LoadController(Path.GetFileName(filePath), Path.GetDirectoryName(filePath));
            }
            var folders = Directory.EnumerateDirectories(dirPath);
            foreach (var folderPath in folders)
            {
                var folderInfo = treeItem.LoadFolder(Path.GetFileName(folderPath), Path.GetDirectoryName(folderPath));
                LoadProjectItems(folderInfo);
            }
        }

        public static bool CheckChildPath(string fatherpath, string childpath)
        {
            if (childpath.Length < fatherpath.Length)
            {
                return false;
            }

            var i = fatherpath.Length;
            var substr = childpath.Substring(0, i);
            return substr == fatherpath;
        }
    }
}