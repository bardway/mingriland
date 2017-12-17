// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Microsoft.Win32;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Dialogs;
#endregion

namespace TLAuto.ControlEx.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Ctors
        public MainViewModel()
        {
            InitNewPorjectCommand();
            InitOpenProjectCommand();
            InitSaveProjectCommand();
            InitAllSaveProjectCommand();
            InitCloseProjectCommand();
            InitClosedCommand();
            InitClosingCommand();
            InitBoardServiceAddressSettingsCommand();
            InitProjectorServiceAddressSettingsCommand();
            InitMusicServiceAddressSettingsCommand();
            InitAppNotificationServiceAddressSettingsCommand();
            InitDMXServiceAddressSettingsCommand();

            TreeVm.OpendProject += TreeVm_OpendProject;

            if (!ConfigHelper.StartUpFilePath.IsNullOrEmpty())
            {
                TreeVm.OpenProject(ConfigHelper.StartUpFilePath);
            }
        }
        #endregion

        #region Methods
        private void TreeVm_OpendProject(object sender, EventArgs e)
        {
            Title = Path.GetFileNameWithoutExtension(TreeVm.Project.FullPath);
        }
        #endregion

        #region Properties
        public TreeViewModel TreeVm { get; } = new TreeViewModel();

        private string _title = "Room控制器";

        public string Title
        {
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
            get => _title;
        }
        #endregion

        #region Event MvvmBindings
        private void InitNewPorjectCommand()
        {
            NewPorjectCommand = new RelayCommand(() =>
                                                 {
                                                     var npd = new NewProjectDialog();
                                                     if (npd.ShowDialog() == true)
                                                     {
                                                         var fileName = npd.FileName;
                                                         var location = npd.Location;
                                                         TreeVm.NewProject(fileName, location);
                                                     }
                                                 });
        }

        public RelayCommand NewPorjectCommand { private set; get; }

        private void InitOpenProjectCommand()
        {
            OpenProjectCommand = new RelayCommand(() =>
                                                  {
                                                      var ofd = new OpenFileDialog();
                                                      ofd.Filter = "Project files(*.tlcproj)|*.tlcproj";
                                                      if (ofd.ShowDialog() == true)
                                                      {
                                                          var fullPath = ofd.FileName;
                                                          TreeVm.OpenProject(fullPath);
                                                      }
                                                  });
        }

        public RelayCommand OpenProjectCommand { private set; get; }

        private void InitSaveProjectCommand()
        {
            SaveProjectCommand = new RelayCommand(() =>
                                                  {
                                                      if (TreeVm.TabSelectedItem != null)
                                                      {
                                                          TreeVm.TabSelectedItem.Save();
                                                      }
                                                      if (TreeVm.Project != null)
                                                      {
                                                          TreeVm.Project.Save();
                                                      }
                                                  });
        }

        public RelayCommand SaveProjectCommand { private set; get; }

        private void InitAllSaveProjectCommand()
        {
            AllSaveProjectCommand = new RelayCommand(() =>
                                                     {
                                                         foreach (var tabTreeItem in TreeVm.TabTreeItems)
                                                         {
                                                             tabTreeItem.Save();
                                                         }
                                                         if (TreeVm.Project != null)
                                                         {
                                                             TreeVm.Project.SaveAll(TreeVm.Project);
                                                         }
                                                     });
        }

        public RelayCommand AllSaveProjectCommand { private set; get; }

        private void InitCloseProjectCommand()
        {
            CloseProjectCommand = new RelayCommand(() => { TreeVm.CloseProject(); });
        }

        public RelayCommand CloseProjectCommand { private set; get; }

        private void InitClosedCommand()
        {
            ClosedCommand = new RelayCommand(() => { TreeVm.CloseProject(); });
        }

        public RelayCommand ClosedCommand { private set; get; }

        private void InitClosingCommand()
        {
            ClosingCommand = new RelayCommand<CancelEventArgs>(e =>
                                                               {
                                                                   if (MessageBox.Show("确认关闭么？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                                                   {
                                                                       e.Cancel = true;
                                                                   }
                                                               });
        }

        public RelayCommand<CancelEventArgs> ClosingCommand { private set; get; }

        private void InitBoardServiceAddressSettingsCommand()
        {
            BoardServiceAddressSettingsCommand = new RelayCommand(() =>
                                                                  {
                                                                      var esa = new EditServiceAddressListDialog(ProjectHelper.Project.ItemXmlInfo.BoardServiceAddressInfos);
                                                                      esa.ShowDialog();
                                                                  });
        }

        public RelayCommand BoardServiceAddressSettingsCommand { private set; get; }

        private void InitProjectorServiceAddressSettingsCommand()
        {
            ProjectorServiceAddressSettingsCommand = new RelayCommand(() =>
                                                                      {
                                                                          var dialog = new EditServiceAddressListDialog(ProjectHelper.Project.ItemXmlInfo.ProjectorGroup.ProjectorServiceAddressInfos);
                                                                          dialog.ShowDialog();
                                                                      });
        }

        public RelayCommand ProjectorServiceAddressSettingsCommand { private set; get; }

        private void InitMusicServiceAddressSettingsCommand()
        {
            MusicServiceAddressSettingsCommand = new RelayCommand(() =>
                                                                  {
                                                                      var esa = new EditServiceAddressListDialog(ProjectHelper.Project.ItemXmlInfo.MusicGroup.MusicServiceAddressInfos);
                                                                      esa.ShowDialog();
                                                                  });
        }

        public RelayCommand MusicServiceAddressSettingsCommand { private set; get; }

        private void InitAppNotificationServiceAddressSettingsCommand()
        {
            AppNotificationServiceAddressSettingsCommand = new RelayCommand(() =>
                                                                            {
                                                                                var esa = new EditServiceAddressListDialog(ProjectHelper.Project.ItemXmlInfo.NotificationGroup.AppNitificationServiceAddressInfos);
                                                                                esa.ShowDialog();
                                                                            });
        }

        public RelayCommand AppNotificationServiceAddressSettingsCommand { private set; get; }

        private void InitDMXServiceAddressSettingsCommand()
        {
            DMXServiceAddressSettingsCommand = new RelayCommand(() =>
                                                                {
                                                                    var esa = new EditServiceAddressListDialog(ProjectHelper.Project.ItemXmlInfo.DMXGroup.DMXServiceAddressInfos);
                                                                    esa.ShowDialog();
                                                                });
        }

        public RelayCommand DMXServiceAddressSettingsCommand { private set; get; }
        #endregion
    }
}