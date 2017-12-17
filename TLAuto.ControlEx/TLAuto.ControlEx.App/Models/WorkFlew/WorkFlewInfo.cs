// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Dialogs;
#endregion

namespace TLAuto.ControlEx.App.Models.WorkFlew
{
    public class WorkFlewInfo : ObservableObject
    {
        private bool _isContinueNextWorkFlews;

        private bool _isEnabledBreakStop = true;

        private bool _isFreeze;

        private volatile bool _isStart;

        private bool _isWarning;

        public WorkFlewInfo()
        {
            InitAddWorkFlewCommand();
            InitInsertWorkFlewCommand();
            InitRemoveWorkFlewCommand();
            InitBreakCommand();
            InitStopCommand();
            InitRetryWorkFlewCommand();
            InitFreezeCommand();
            WorkFlews.CollectionChanged += WorkFlews_CollectionChanged;
        }

        public string CID { set; get; }

        //public string  

        [XmlIgnore]
        public WorkFlewInfo Parent { set; get; }

        public bool IsFreeze
        {
            set
            {
                _isFreeze = value;
                RaisePropertyChanged();
            }
            get => _isFreeze;
        }

        [XmlIgnore]
        public bool IsWarning
        {
            set
            {
                _isWarning = value;
                RaisePropertyChanged();
            }
            get => _isWarning;
        }

        [XmlIgnore]
        public bool IsStart
        {
            private set
            {
                _isStart = value;
                RaisePropertyChanged();
            }
            get => _isStart;
        }

        [XmlIgnore]
        public bool IsEnabledBreakStop
        {
            set
            {
                _isEnabledBreakStop = value;
                RaisePropertyChanged();
            }
            get => _isEnabledBreakStop;
        }

        public ObservableCollection<WorkFlewInfo> WorkFlews { get; } = new ObservableCollection<WorkFlewInfo>();

        private void WriteMsg(string msg)
        {
            Messenger.Default.Send(new NotificationMessage(msg));
        }

        private void WriteMsg(string msg, MessageBoxResult status)
        {
            Messenger.Default.Send(new NotificationMessage(status, msg));
        }

        private void WorkFlews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (WorkFlewInfo flewInfo in e.NewItems)
                {
                    flewInfo.Parent = this;
                }
            }
        }

        private void RaiseStopEvent(UIElement element)
        {
            IsStart = false;
            IsWarning = false;
            DispatcherHelper.CheckBeginInvokeOnUI(() => { element.RaiseEvent(new RoutedEventArgs(WorkFlewStopEvent, this)); });
        }

        public async void RunWorkFlews(UIElement element, bool isContinueNextWorkFlews)
        {
            _isContinueNextWorkFlews = isContinueNextWorkFlews;
            var result = await RunFlew();
            if (!result)
            {
                IsWarning = true;
                return;
            }
            if (!isContinueNextWorkFlews || (WorkFlews.Count == 0))
            {
                RaiseStopEvent(element);
            }
            else
            {
                if (WorkFlews.Count == 0)
                {
                    RaiseStopEvent(element);
                    return;
                }
                Parallel.ForEach(WorkFlews, workFlew => workFlew.RunWorkFlews(element, true));
            }
        }

        private async Task<bool> RunFlew()
        {
            if (IsFreeze)
            {
                return true;
            }
            if (IsStart)
            {
                return false;
            }
            IsWarning = false;
            var resultForTask = false;
            var controllerXmlInfo = ProjectHelper.GetControllerXmlInfo(ProjectHelper.Project, CID);
            WriteMsg(string.Format("正在运行任务，描述为：{0}", controllerXmlInfo.Description));
            IsStart = true;
            var result = true;
            try
            {
                foreach (var controllerExcute in controllerXmlInfo.Excutes)
                {
                    resultForTask = await controllerExcute.Excute(WriteMsg);
                    if (!resultForTask)
                    {
                        break;
                    }
                }
                if (!resultForTask)
                {
                    WriteMsg(string.Format("运行失败，描述为：{0}", controllerXmlInfo.Description), MessageBoxResult.Cancel);
                    result = false;
                }
                else
                {
                    WriteMsg(string.Format("运行成功，描述为：{0}", controllerXmlInfo.Description));
                }
            }
            catch (OperationCanceledException oc)
            {
                WriteMsg(string.Format("运行失败，原因为{0}，描述为：{1}", oc.Message, controllerXmlInfo.Description), MessageBoxResult.Cancel);
                result = false;
            }
            catch (Exception ex)
            {
                WriteMsg(string.Format("运行失败，原因为{0}，描述为：{1}", ex.Message, controllerXmlInfo.Description), MessageBoxResult.Cancel);
                result = false;
            }
            IsStart = false;
            return result;
        }

        private ControllerXmlInfo StopControllerExcute(bool isBreak)
        {
            var controllerXmlInfo = ProjectHelper.GetControllerXmlInfo(ProjectHelper.Project, CID);
            foreach (var controllerExcute in controllerXmlInfo.Excutes)
            {
                if (isBreak)
                {
                    controllerExcute.BreakExcute(WriteMsg);
                }
                else
                {
                    controllerExcute.StopExcute(WriteMsg);
                }
            }
            return controllerXmlInfo;
        }

        #region Routed Events
        #region WorkFlewStop
        public static readonly RoutedEvent WorkFlewStopEvent =
            EventManager.RegisterRoutedEvent("WorkFlewStop", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WorkFlewInfo));

        public static void AddWorkFlewStopHandler(DependencyObject d, RoutedEventHandler h)
        {
            var e = d as UIElement;
            if (e != null)
            {
                e.AddHandler(WorkFlewStopEvent, h);
            }
        }

        public static void RemoveWorkFlewStopHandler(DependencyObject d, RoutedEventHandler h)
        {
            var e = d as UIElement;
            if (e != null)
            {
                e.RemoveHandler(WorkFlewStopEvent, h);
            }
        }
        #endregion
        #endregion

        #region Event MvvmBingdings
        private void InitAddWorkFlewCommand()
        {
            AddWorkFlewCommand = new RelayCommand(() =>
                                                  {
                                                      var dialog = new AddWorkFlewDialog();
                                                      if (dialog.ShowDialog() == true)
                                                      {
                                                          var workFlewInfo = new WorkFlewInfo {CID = dialog.CID};
                                                          WorkFlews.Add(workFlewInfo);
                                                      }
                                                  });
        }

        [XmlIgnore]
        public RelayCommand AddWorkFlewCommand { private set; get; }

        private void InitInsertWorkFlewCommand()
        {
            InsertWorkFlewCommand = new RelayCommand(() =>
                                                     {
                                                         var dialog = new AddWorkFlewDialog();
                                                         if (dialog.ShowDialog() == true)
                                                         {
                                                             var workFlewInfo = new WorkFlewInfo {CID = dialog.CID};
                                                             var insertInfos = WorkFlews.ToList();
                                                             WorkFlews.Clear();
                                                             WorkFlews.Add(workFlewInfo);
                                                             foreach (var info in insertInfos)
                                                             {
                                                                 info.Parent = workFlewInfo;
                                                                 workFlewInfo.WorkFlews.Add(info);
                                                             }
                                                         }
                                                     });
        }

        [XmlIgnore]
        public RelayCommand InsertWorkFlewCommand { private set; get; }

        private void InitRemoveWorkFlewCommand()
        {
            RemoveWorkFlewCommand = new RelayCommand(() =>
                                                     {
                                                         if (MessageBox.Show("确认删除工作流程吗？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                                         {
                                                             if (Parent == null)
                                                             {
                                                                 OnParentRemoved();
                                                                 return;
                                                             }
                                                             var parentWorkFlews = Parent.WorkFlews;
                                                             var indexOf = parentWorkFlews.IndexOf(this);
                                                             parentWorkFlews.RemoveAt(indexOf);
                                                             foreach (var flewInfo in WorkFlews)
                                                             {
                                                                 parentWorkFlews.Insert(indexOf, flewInfo);
                                                                 flewInfo.Parent = Parent;
                                                                 indexOf++;
                                                             }
                                                         }
                                                     });
        }

        [XmlIgnore]
        public RelayCommand RemoveWorkFlewCommand { private set; get; }

        private void InitBreakCommand()
        {
            BreakCommand = new RelayCommand(() =>
                                            {
                                                IsEnabledBreakStop = false;
                                                var controllerXmlInfo = StopControllerExcute(true);
                                                WriteMsg(string.Format("任务跳过成功，描述为：{0}", controllerXmlInfo.Description));
                                                IsEnabledBreakStop = true;
                                            });
        }

        private void InitRetryWorkFlewCommand()
        {
            RetryWorkFlewCommand = new RelayCommand<RoutedEventArgs>(e => { RunWorkFlews((UIElement)e.Source, _isContinueNextWorkFlews); });
        }

        [XmlIgnore]
        public RelayCommand<RoutedEventArgs> RetryWorkFlewCommand { private set; get; }

        [XmlIgnore]
        public RelayCommand BreakCommand { private set; get; }

        private void InitStopCommand()
        {
            StopCommand = new RelayCommand<RoutedEventArgs>(e =>
                                                            {
                                                                IsEnabledBreakStop = false;
                                                                var controllerXmlInfo = StopControllerExcute(false);
                                                                WriteMsg(string.Format("任务停止成功，描述为：{0}", controllerXmlInfo.Description));
                                                                RaiseStopEvent((UIElement)e.Source);
                                                                IsEnabledBreakStop = true;
                                                            });
        }

        [XmlIgnore]
        public RelayCommand<RoutedEventArgs> StopCommand { private set; get; }

        private void InitFreezeCommand()
        {
            FreezeCommand = new RelayCommand(() => { IsFreeze = !IsFreeze; });
        }

        [XmlIgnore]
        public RelayCommand FreezeCommand { private set; get; }
        #endregion

        #region Events
        public event EventHandler ParentRemoved;

        protected virtual void OnParentRemoved()
        {
            ParentRemoved?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}