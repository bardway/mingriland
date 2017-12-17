// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Windows;

using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Models;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// AddWorkFlewDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddWorkFlewDialog : Window
    {
        public AddWorkFlewDialog()
        {
            InitializeComponent();
            InitTv();
            Closed += AddWorkFlewDialog_Closed;
        }

        public string CID { private set; get; }

        private void AddWorkFlewDialog_Closed(object sender, EventArgs e)
        {
            Tv.ItemsSource = null;
        }

        private void InitTv()
        {
            //var projectInfo = ProjectHelper.GetProjectInfo(ProjectHelper.Project.HeaderName, ProjectHelper.Project.DirPath);
            //projectInfo.IsExpanded = true;
            Tv.ItemsSource = new List<ProjectInfo> {ProjectHelper.Project};
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var controller = Tv.SelectedItem as ControllerInfo;
            if (controller != null)
            {
                CID = controller.ItemXmlInfo.CID;
                DialogResult = true;
                Close();
            }
        }
    }
}