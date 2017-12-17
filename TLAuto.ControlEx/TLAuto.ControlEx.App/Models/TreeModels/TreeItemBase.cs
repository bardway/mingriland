// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Base.Extensions;
using TLAuto.BaseEx.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Controls.Models.Events;
#endregion

namespace TLAuto.ControlEx.App.Models.TreeModels
{
    public abstract class TreeItemBase : ViewModelBase
    {
        #region Ctors
        public TreeItemBase(TreeItemBase parent, string fileName, string location)
        {
            Parent = parent;
            HeaderName = fileName;
            DirPath = location;
            InitTabItemCloseCommand();
            InitTabItemTitleMouseDownCommand();
            InitOpenExplorerCommand();
            InitNewControllerCommand();
            InitNewFolderCommand();
            InitDeleteForMenuCommand();
            InitDeleteForShortcutCommand();
            InitRenameCommand();
            InitRenameTextChanged();
            InitExpanderAllCommand();
            InitCollapsedAllCommand();
            InitRunCommand();
        }
        #endregion

        #region Routed Events
        #region TabItemClosed
        public static readonly RoutedEvent TabItemClosedEvent =
            EventManager.RegisterRoutedEvent("TabItemClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeItemBase));

        public static void AddTabItemClosedHandler(DependencyObject d, RoutedEventHandler h)
        {
            var e = d as UIElement;
            if (e != null)
            {
                e.AddHandler(TabItemClosedEvent, h);
            }
        }

        public static void RemoveTabItemClosedHandler(DependencyObject d, RoutedEventHandler h)
        {
            var e = d as UIElement;
            if (e != null)
            {
                e.RemoveHandler(TabItemClosedEvent, h);
            }
        }
        #endregion
        #endregion

        #region Methods
        public override void Cleanup()
        {
            foreach (var treeItemBase in Items)
            {
                treeItemBase.Cleanup();
            }
            Items.Clear();
            base.Cleanup();
        }

        protected void LoadItemXml<T>(string fullPath) where T: class
        {
            if (File.Exists(fullPath))
            {
                ItemXml = fullPath.ToObjectFromXmlFile<TreeItemXmlBase>();
            }
            else
            {
                ItemXml = Activator.CreateInstance(typeof(T));
            }
        }

        public virtual void Save()
        {
            if (ItemXml != null)
            {
                try
                {
                    ((TreeItemXmlBase)ItemXml).ToXmlFile(FullPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("{0} 保存出错，原因：{1}", HeaderName, ex.Message));
                }
            }
        }

        public void SaveAll(TreeItemBase parent)
        {
            parent.Save();
            foreach (var item in parent.Items)
            {
                SaveAll(item);
            }
        }

        public void OpenExplorer()
        {
            Process.Start("Explorer.exe", Path.HasExtension(FullPath) ? "/select," + FullPath : FullPath);
        }

        private void Rename()
        {
            IsEditHeaderName = true;
        }

        public bool NewController(out ControllerInfo controllerInfo)
        {
            controllerInfo = null;
            var dirPath = TreeItemHelper.GetDirPath(FullPath);
            var controllerName = TreeItemHelper.GetCreateControllerName("Controller", ControllerInfo.EXTENSION, dirPath);
            var controllerPath = Path.Combine(dirPath, controllerName);
            if (!File.Exists(controllerPath))
            {
                controllerInfo = LoadController(controllerName, dirPath);
                IsExpanded = true;
                controllerInfo.Save();
                return true;
            }
            return false;
        }

        internal ControllerInfo LoadController(string fileName, string dirPath)
        {
            var controllerInfo = new ControllerInfo(this, fileName, dirPath);
            Items.Add(controllerInfo);
            return controllerInfo;
        }

        public bool NewFolder(out FolderInfo folderInfo)
        {
            folderInfo = null;
            var dirPath = TreeItemHelper.GetDirPath(FullPath);
            var folderName = TreeItemHelper.GetCreateFolderName("NewFolder", dirPath);
            var folderPath = Path.Combine(dirPath, folderName);
            if (Directory.CreateDirectory(folderPath).Exists)
            {
                folderInfo = LoadFolder(folderName, dirPath);
                IsExpanded = true;
                return true;
            }
            return false;
        }

        internal FolderInfo LoadFolder(string folderName, string dirPath)
        {
            var folderInfo = new FolderInfo(this, folderName, dirPath);
            Items.Add(folderInfo);
            return folderInfo;
        }

        private void Delete(UIElement element)
        {
            if (MessageBox.Show("确定要删除吗？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                RaiseTabColose(element);
                try
                {
                    if (Path.HasExtension(FullPath))
                    {
                        File.Delete(FullPath);
                    }
                    else
                    {
                        Directory.Delete(FullPath, true);
                    }
                    if (Parent != null)
                    {
                        Cleanup();
                        Parent.Items.Remove(this);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("删除出错，错误为：" + ex);
                }
            }
        }

        private void RaiseTabColose(UIElement element)
        {
            element.RaiseEvent(new RoutedEventArgs(TabItemClosedEvent, this));
        }

        protected virtual bool Rename(string changeText, string oldText)
        {
            return false;
        }

        public void MoveTo(TreeItemBase target)
        {
            var hasEx = Path.HasExtension(target.FullPath);
            var targetDirPath = hasEx ? target.DirPath : target.FullPath;
            var result = MoveTo(targetDirPath);
            if (result)
            {
                Parent.Items.Remove(this);
                target.Items.Add(this);
                Parent = target;
                UpdatePathWithChildPaths(this);
                Parent.IsExpanded = true;
            }
        }

        protected virtual bool MoveTo(string desPath)
        {
            return false;
        }

        private static void UpdatePathWithChildPaths(TreeItemBase treeItemBase)
        {
            var parentPath = Path.HasExtension(treeItemBase.Parent.FullPath) ? treeItemBase.Parent.DirPath : treeItemBase.Parent.FullPath;
            treeItemBase.DirPath = parentPath;
            foreach (var treeItem in treeItemBase.Items)
            {
                UpdatePathWithChildPaths(treeItem);
            }
        }

        private static void ExpanderAll(TreeItemBase treeItem)
        {
            treeItem.IsExpanded = true;
            foreach (var treeItemBase in treeItem.Items)
            {
                ExpanderAll(treeItemBase);
            }
        }

        private static void CollapsedAll(TreeItemBase treeItem)
        {
            treeItem.IsExpanded = false;
            foreach (var treeItemBase in treeItem.Items)
            {
                CollapsedAll(treeItemBase);
            }
        }

        protected virtual void Run() { }
        #endregion

        #region Properties
        public TreeItemBase Parent { set; get; }

        public object ItemXml { private set; get; }

        public ObservableCollection<TreeItemBase> Items { get; } = new ObservableCollection<TreeItemBase>();

        private string _headerName;

        public string HeaderName
        {
            set
            {
                _headerName = value;
                RaisePropertyChanged();
            }
            get => _headerName;
        }

        private bool _isEditHeaderName;

        public bool IsEditHeaderName
        {
            set
            {
                _isEditHeaderName = value;
                RaisePropertyChanged();
            }
            get => _isEditHeaderName;
        }

        public string DirPath { private set; get; }

        public string FullPath => Path.Combine(DirPath, HeaderName);

        private bool _isSelected;

        public bool IsSelected
        {
            set
            {
                _isSelected = value;
                RaisePropertyChanged();
            }
            get => _isSelected;
        }

        private bool _isTabSelected;

        public bool IsTabSelected
        {
            set
            {
                _isTabSelected = value;
                RaisePropertyChanged();
            }
            get => _isTabSelected;
        }

        private bool _isExpanded;

        public bool IsExpanded
        {
            set
            {
                _isExpanded = value;
                RaisePropertyChanged();
            }
            get => _isExpanded;
        }

        public abstract bool HasTabSettingsItem { get; }
        #endregion

        #region Event MvvmBindings
        private void InitTabItemCloseCommand()
        {
            TabItemCloseCommand = new RelayCommand<RoutedEventArgs>(e => { CloseTabItem((UIElement)e.Source); });
        }

        private void CloseTabItem(UIElement element)
        {
            RaiseTabColose(element);
            Save();
        }

        public RelayCommand<RoutedEventArgs> TabItemCloseCommand { private set; get; }

        private void InitTabItemTitleMouseDownCommand()
        {
            TabItemTitleMouseDownCommand = new RelayCommand<MouseButtonEventArgs>(e =>
                                                                                  {
                                                                                      if (e.ChangedButton == MouseButton.Middle)
                                                                                      {
                                                                                          CloseTabItem((UIElement)e.Source);
                                                                                      }
                                                                                  });
        }

        public RelayCommand<MouseButtonEventArgs> TabItemTitleMouseDownCommand { private set; get; }

        #region Menu
        private void InitOpenExplorerCommand()
        {
            OpenExplorerCommand = new RelayCommand(OpenExplorer);
        }

        public RelayCommand OpenExplorerCommand { private set; get; }

        private void InitNewControllerCommand()
        {
            NewControllerCommand = new RelayCommand(() =>
                                                    {
                                                        ControllerInfo controllerInfo;
                                                        if (NewController(out controllerInfo))
                                                        {
                                                            controllerInfo.IsSelected = true;
                                                            controllerInfo.Rename();
                                                        }
                                                    });
        }

        public RelayCommand NewControllerCommand { private set; get; }

        private void InitNewFolderCommand()
        {
            NewFolderCommand = new RelayCommand(() =>
                                                {
                                                    FolderInfo folderInfo;
                                                    if (NewFolder(out folderInfo))
                                                    {
                                                        folderInfo.IsSelected = true;
                                                        folderInfo.Rename();
                                                    }
                                                });
        }

        public RelayCommand NewFolderCommand { private set; get; }

        private void InitDeleteForMenuCommand()
        {
            DeleteForMenuCommand = new RelayCommand<RoutedEventArgs>(e =>
                                                                     {
                                                                         var menuItem = (MenuItem)e.Source;
                                                                         var contextMenu = menuItem.FindLogicalParent<ContextMenu>();
                                                                         Delete(contextMenu.PlacementTarget);
                                                                     });
        }

        public RelayCommand<RoutedEventArgs> DeleteForMenuCommand { private set; get; }

        private void InitDeleteForShortcutCommand()
        {
            DeleteForShortcutCommand = new RelayCommand<UIElement>(Delete);
        }

        public RelayCommand<UIElement> DeleteForShortcutCommand { private set; get; }

        private void InitRenameCommand()
        {
            RenameCommand = new RelayCommand(Rename);
        }

        public RelayCommand RenameCommand { private set; get; }

        private void InitRenameTextChanged()
        {
            RenameTextChanged = new RelayCommand<EditTextChangedRoutedEventArgs>(e =>
                                                                                 {
                                                                                     if (e.EditText == e.OldText)
                                                                                     {
                                                                                         return;
                                                                                     }
                                                                                     if (!Rename(e.EditText, e.OldText))
                                                                                     {
                                                                                         HeaderName = e.OldText;
                                                                                         IsEditHeaderName = true;
                                                                                     }
                                                                                     else
                                                                                     {
                                                                                         UpdatePathWithChildPaths(this);
                                                                                     }
                                                                                 });
        }

        public RelayCommand<EditTextChangedRoutedEventArgs> RenameTextChanged { private set; get; }

        private void InitExpanderAllCommand()
        {
            ExpanderAllCommand = new RelayCommand(() => { ExpanderAll(this); });
        }

        public RelayCommand ExpanderAllCommand { private set; get; }

        private void InitCollapsedAllCommand()
        {
            CollapsedAllCommand = new RelayCommand(() => { CollapsedAll(this); });
        }

        public RelayCommand CollapsedAllCommand { private set; get; }

        private void InitRunCommand()
        {
            RunCommand = new RelayCommand(Run);
        }

        public RelayCommand RunCommand { private set; get; }
        #endregion
        #endregion
    }
}