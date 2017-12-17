// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.BaseEx.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Models;
using TLAuto.ControlEx.App.Models.TreeModels;
#endregion

namespace TLAuto.ControlEx.App.ViewModels
{
    public class TreeViewModel : ViewModelBase
    {
        #region Ctors
        public TreeViewModel()
        {
            InitSelectedItemChangedCommand();
            InitMouseDoubleClickCommand();
            InitTabSelectedItemChangedCommand();
            InitTabItemClosedCommand();
        }
        #endregion

        #region Methods
        public void NewProject(string fileName, string location)
        {
            if (Project == null)
            {
                location = Path.Combine(location, fileName);
                if (!Directory.Exists(location))
                {
                    Directory.CreateDirectory(location);
                }
                AddProject(fileName, location);
                if (Project != null)
                {
                    Project.Save();
                }
            }
        }

        public void SaveProject()
        {
            foreach (var treeItemBase in TabTreeItems)
            {
                treeItemBase.Save();
            }
        }

        private void AddProject(string fileName, string location)
        {
            var projectInfo = ProjectHelper.GetProjectInfo(fileName, location);
            projectInfo.IsExpanded = true;
            Project = projectInfo;
            ProjectHelper.InitProject(projectInfo);
            HasProject = true;
            OnOpendProject();
        }

        public void OpenProject(string fullPath)
        {
            if (Project != null)
            {
                CloseProject();
            }
            var fileName = Path.GetFileNameWithoutExtension(fullPath);
            var location = Path.GetDirectoryName(fullPath);
            AddProject(fileName, location);
        }

        public void CloseProject()
        {
            if (Project != null)
            {
                SaveProject();
                SelectedItem = null;
                TabSelectedItem = null;
                TabTreeItems.Clear();
                Project.Cleanup();
                Project = null;
                ProjectHelper.CloseProject();
                HasProject = false;
            }
        }
        #endregion

        #region Properties
        private ProjectInfo _project;

        public ProjectInfo Project
        {
            private set
            {
                _project = value;
                RaisePropertyChanged();
            }
            get => _project;
        }

        private TreeItemBase _selectedItem;

        public TreeItemBase SelectedItem
        {
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
            get => _selectedItem;
        }

        private TreeItemBase _tabSelectedItem;

        public TreeItemBase TabSelectedItem
        {
            set
            {
                _tabSelectedItem = value;
                RaisePropertyChanged();
            }
            get => _tabSelectedItem;
        }

        public ObservableCollection<TreeItemBase> TabTreeItems { get; } = new ObservableCollection<TreeItemBase>();

        private bool _hasProject;

        public bool HasProject
        {
            private set
            {
                _hasProject = value;
                RaisePropertyChanged();
            }
            get => _hasProject;
        }
        #endregion

        #region Event MvvmBindings
        private void InitSelectedItemChangedCommand()
        {
            SelectedItemChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(e => { SelectedItem = (TreeItemBase)e.NewValue; });
        }

        public RelayCommand<RoutedPropertyChangedEventArgs<object>> SelectedItemChangedCommand { private set; get; }

        private void InitMouseDoubleClickCommand()
        {
            PreviewMouseDoubleClickCommand = new RelayCommand<MouseButtonEventArgs>(e =>
                                                                                    {
                                                                                        var element = e.OriginalSource as DependencyObject;
                                                                                        if (element != null)
                                                                                        {
                                                                                            var treeViewItem = element.FindVisualParent<TreeViewItem>();
                                                                                            if ((treeViewItem != null) && (treeViewItem.DataContext == SelectedItem))
                                                                                            {
                                                                                                if (SelectedItem.HasTabSettingsItem)
                                                                                                {
                                                                                                    if (TabTreeItems.FirstOrDefault(t => t == SelectedItem) == null)
                                                                                                    {
                                                                                                        TabTreeItems.Add(SelectedItem);
                                                                                                    }
                                                                                                    SelectedItem.IsTabSelected = true;
                                                                                                    e.Handled = true;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    });
        }

        public RelayCommand<MouseButtonEventArgs> PreviewMouseDoubleClickCommand { private set; get; }

        private void InitTabSelectedItemChangedCommand()
        {
            TabSelectedItemChangedCommand = new RelayCommand<SelectionChangedEventArgs>(e =>
                                                                                        {
                                                                                            foreach (var treeItemBase in e.AddedItems)
                                                                                            {
                                                                                                var selectedItem = treeItemBase as TreeItemBase;
                                                                                                if (selectedItem != null)
                                                                                                {
                                                                                                    TabSelectedItem = selectedItem;
                                                                                                }
                                                                                            }
                                                                                        });
        }

        public RelayCommand<SelectionChangedEventArgs> TabSelectedItemChangedCommand { private set; get; }

        private void InitTabItemClosedCommand()
        {
            TabItemClosedCommand = new RelayCommand<RoutedEventArgs>(e =>
                                                                     {
                                                                         var treeItemBase = (TreeItemBase)e.OriginalSource;
                                                                         var removeItems = TabTreeItems.Where(tabTreeItem => TreeItemHelper.CheckChildPath(treeItemBase.FullPath, tabTreeItem.FullPath)).ToList();
                                                                         foreach (var removeItem in removeItems)
                                                                         {
                                                                             TabTreeItems.Remove(removeItem);
                                                                         }
                                                                     });
        }

        public RelayCommand<RoutedEventArgs> TabItemClosedCommand { private set; get; }
        #endregion

        #region Events
        public event EventHandler OpendProject;

        protected virtual void OnOpendProject()
        {
            OpendProject?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}