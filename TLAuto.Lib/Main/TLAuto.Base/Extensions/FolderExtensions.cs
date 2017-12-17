// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
#endregion

namespace TLAuto.Base.Extensions
{
    public static class FolderExtensions
    {
        /// <summary>
        /// 计算文件大小函数(保留两位小数),Size为字节大小
        /// </summary>
        /// <param name="size">初始文件大小</param>
        /// <returns></returns>
        public static string ConvertSize(this long size)
        {
            var mStrSize = "";
            var factSize = size;
            if (factSize < 1024.00)
            {
                mStrSize = factSize.ToString("F2") + " Byte";
            }
            else
            {
                if ((factSize >= 1024.00) && (factSize < 1048576))
                {
                    mStrSize = (factSize / 1024.00).ToString("F2") + " K";
                }
                else
                {
                    if ((factSize >= 1048576) && (factSize < 1073741824))
                    {
                        mStrSize = (factSize / 1024.00 / 1024.00).ToString("F2") + " M";
                    }
                    else
                    {
                        if (factSize >= 1073741824)
                        {
                            mStrSize = (factSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
                        }
                    }
                }
            }
            return mStrSize;
        }

        /// <summary>
        /// b -> M
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double ConvertBytesToGegabytes(this long bytes)
        {
            return bytes / 1024f / 1024f / 1024f;
        }

        /// <summary>
        /// kb-> M
        /// </summary>
        /// <param name="kilobytes"></param>
        /// <returns></returns>
        public static double ConvertKilobytesToMegabytes(this long kilobytes)
        {
            return kilobytes / 1024f;
        }

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <returns></returns>
        public static long GetDirectoryLength(this string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                return 0;
            }
            var di = new DirectoryInfo(dirPath);
            var len = di.GetFiles().Sum(fi => fi.Length);
            var dis = di.GetDirectories();
            if (dis.Length > 0)
            {
                len += dis.Sum(t => GetDirectoryLength(t.FullName));
            }
            return len;
        }

        /// <summary>
        /// 获取文件夹大小,并返回其包含的所有文件
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <param name="files">文件路径列表集合</param>
        /// <param name="recursive">若需要 path 中的文件、子目录的文件，则为 true；否则为 false。</param>
        /// <returns></returns>
        public static long GetDirectoryLength(this string dirPath, ref List<string> files, bool recursive = false)
        {
            if (!Directory.Exists(dirPath))
            {
                return 0;
            }
            var di = new DirectoryInfo(dirPath);
            var fileInfos = di.GetFiles();
            long len = 0;
            foreach (var fileInfo in fileInfos)
            {
                files.Add(fileInfo.FullName);
                len += fileInfo.Length;
            }
            if (recursive)
            {
                var dis = di.GetDirectories();
                if (dis.Length > 0)
                {
                    foreach (var directoryInfo in dis)
                    {
                        len += GetDirectoryLength(directoryInfo.FullName, ref files, recursive);
                    }
                }
            }
            return len;
        }

        /// <summary>
        /// 打开资源管理器
        /// </summary>
        /// <param name="path">路径</param>
        public static void OpenExplorer(this string path)
        {
            Process.Start("explorer.exe", path);
        }

        /// <summary>
        /// /// 打开资源管理器并选中指定文件或文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void OpenExplorerWithSelectFileOrFolder(this string path)
        {
            var proc = new Process();
            proc.StartInfo.FileName = "explorer";
            proc.StartInfo.Arguments = @"/select," + path;
            proc.Start();
        }

        /// <summary>
        /// 验证文件名是否合法
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>true为合法；False为非法</returns>
        public static bool ValidateFileName(this string filename)
        {
            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                return false;
            }
            return true;
        }
    }
}