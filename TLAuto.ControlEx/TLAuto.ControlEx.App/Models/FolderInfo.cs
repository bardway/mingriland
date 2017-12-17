// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.IO;
using System.Windows;

using TLAuto.ControlEx.App.Models.TreeModels;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class FolderInfo : TreeItemBase
    {
        public FolderInfo(TreeItemBase parent, string fileName, string location)
            : base(parent, fileName, location) { }

        public override bool HasTabSettingsItem => false;

        protected override bool Rename(string changeText, string oldText)
        {
            var changeFullPath = Path.Combine(DirPath, changeText);
            if (Directory.Exists(changeFullPath))
            {
                MessageBox.Show("已存在相同文件名。");
            }
            else
            {
                var oldFullPath = Path.Combine(DirPath, oldText);
                Directory.Move(oldFullPath, changeFullPath);
                return true;
            }
            return false;
        }

        protected override bool MoveTo(string desPath)
        {
            try
            {
                var srcInfo = new DirectoryInfo(FullPath);
                var desFolderPath = Path.Combine(desPath, srcInfo.Name);
                srcInfo.MoveTo(desFolderPath);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }
    }
}