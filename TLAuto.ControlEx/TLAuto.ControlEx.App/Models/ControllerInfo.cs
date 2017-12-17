// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.IO;
using System.Windows;

using GalaSoft.MvvmLight.Messaging;

using TLAuto.ControlEx.App.Models.TreeModels;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class ControllerInfo : TreeItemBase
    {
        public const string EXTENSION = "tlcs";

        public ControllerInfo(TreeItemBase parent, string fileName, string location)
            : base(parent, fileName, location)
        {
            LoadItemXml<ControllerXmlInfo>(FullPath);
        }

        public ControllerXmlInfo ItemXmlInfo => (ControllerXmlInfo)ItemXml;

        public override bool HasTabSettingsItem => true;

        protected override bool Rename(string changeText, string oldText)
        {
            var extension = Path.GetExtension(changeText);
            if (extension != "." + EXTENSION)
            {
                MessageBox.Show("不能更改扩展名。");
            }
            else
            {
                var changeFullPath = Path.Combine(DirPath, changeText);
                if (File.Exists(changeFullPath))
                {
                    MessageBox.Show("已存在相同文件名。");
                }
                else
                {
                    var oldFullPath = Path.Combine(DirPath, oldText);
                    File.Move(oldFullPath, changeFullPath);
                    return true;
                }
            }
            return false;
        }

        protected override bool MoveTo(string desPath)
        {
            try
            {
                var newFullPath = Path.Combine(desPath, HeaderName);
                if (FullPath == newFullPath)
                {
                    MessageBox.Show("源路径与目标路径必须不同。");
                }
                else
                {
                    File.Move(FullPath, newFullPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        private void WriteMsg(string msg)
        {
            Messenger.Default.Send(new NotificationMessage(msg));
        }

        protected override async void Run()
        {
            foreach (var controllerExcute in ItemXmlInfo.Excutes)
            {
                await controllerExcute.Excute(WriteMsg);
            }
        }
    }
}