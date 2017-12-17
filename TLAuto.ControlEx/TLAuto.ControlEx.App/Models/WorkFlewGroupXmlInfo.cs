// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Dialogs;
using TLAuto.ControlEx.App.Models.Enums;
using TLAuto.ControlEx.App.Models.WorkFlew;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class WorkFlewGroupXmlInfo : ObservableObject
    {
        private readonly Timer _timer = new Timer(1000);

        private bool _isExpanded = true;

        private bool _isStartEnabled = true;

        private WorkFlewInfo _selectedWorkFlewItem;

        private string _startTimeDurationText;

        private string _startTimeText;

        private WorkFlewGroupType _workFlewGroupType;

        public WorkFlewGroupXmlInfo()
        {
            InitAddWorkFlewCommand();
            InitInsertWorkFlewCommand();
            InitStartandContinueWorkFlewCommand();
            InitSelectedItemChangedCommand();
            InitWorkFlewStopCommand();
            InitStartWorkFlewCommand();
            InitStartTimeClickCommand();
            InitStopTimeClickCommand();
            InitCheckedPauseCommand();
            InitUncheckedPauseCommand();
            WorkFlews.CollectionChanged += WorkFlews_CollectionChanged;
            _timer.Elapsed += Timer_Elapsed;
        }

        public bool IsExpanded
        {
            set
            {
                _isExpanded = value;
                RaisePropertyChanged();
            }
            get => _isExpanded;
        }

        public WorkFlewGroupType WorkFlewGroupType
        {
            set
            {
                _workFlewGroupType = value;
                RaisePropertyChanged();
            }
            get => _workFlewGroupType;
        }

        public ObservableCollection<WorkFlewInfo> WorkFlews { get; } = new ObservableCollection<WorkFlewInfo>();

        [XmlIgnore]
        public WorkFlewInfo SelectedWorkFlewItem
        {
            set
            {
                _selectedWorkFlewItem = value;
                RaisePropertyChanged();
            }
            get => _selectedWorkFlewItem;
        }

        [XmlIgnore]
        public bool IsStartEnabled
        {
            set
            {
                _isStartEnabled = value;
                RaisePropertyChanged();
            }
            get => _isStartEnabled;
        }

        [XmlIgnore]
        public string StartTimeText
        {
            set
            {
                _startTimeText = value;
                RaisePropertyChanged();
            }
            get => _startTimeText;
        }

        [XmlIgnore]
        public string StartTimeDurationText
        {
            set
            {
                _startTimeDurationText = value;
                RaisePropertyChanged();
            }
            get => _startTimeDurationText;
        }

        private void WorkFlews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (WorkFlewInfo flewInfo in e.NewItems)
                {
                    flewInfo.ParentRemoved += FlewInfo_ParentRemoved;
                }
            }
        }

        private void FlewInfo_ParentRemoved(object sender, EventArgs e)
        {
            var removeInfo = (WorkFlewInfo)sender;
            var parentWorkFlews = WorkFlews;
            var indexOf = parentWorkFlews.IndexOf(removeInfo);
            parentWorkFlews.RemoveAt(indexOf);
            foreach (var flewInfo in removeInfo.WorkFlews)
            {
                parentWorkFlews.Insert(indexOf, flewInfo);
                flewInfo.Parent = null;
                indexOf++;
            }
        }

        private bool HasRunWorkFlew(IEnumerable<WorkFlewInfo> workFlews)
        {
            foreach (var workFlewInfo in workFlews)
            {
                if (workFlewInfo.IsStart)
                {
                    return true;
                }
                return HasRunWorkFlew(workFlewInfo.WorkFlews);
            }
            return false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime startTime;
            if (DateTime.TryParse(StartTimeText, out startTime))
            {
                var ts = DateTime.Now - startTime;
                StartTimeDurationText = $"{ts.TotalMinutes.ToInt32()}:{ts.Seconds}";
            }
        }

        private void StopTime()
        {
            _timer.Stop();
            StartTimeText = string.Empty;
            StartTimeDurationText = "0";
        }

        #region Event MvvmBindings
        private void InitAddWorkFlewCommand()
        {
            AddWorkFlewCommand = new RelayCommand(() =>
                                                  {
                                                      var dialog = new AddWorkFlewDialog();
                                                      if (dialog.ShowDialog() == true)
                                                      {
                                                          WorkFlews.Add(new WorkFlewInfo {CID = dialog.CID});
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

        private void InitSelectedItemChangedCommand()
        {
            SelectedItemChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(e => { SelectedWorkFlewItem = (WorkFlewInfo)e.NewValue; });
        }

        [XmlIgnore]
        public RelayCommand<RoutedPropertyChangedEventArgs<object>> SelectedItemChangedCommand { private set; get; }

        private void InitStartandContinueWorkFlewCommand()
        {
            StartandContinueWorkFlewCommand = new RelayCommand<RoutedEventArgs>(e =>
                                                                                {
                                                                                    if (SelectedWorkFlewItem == null)
                                                                                    {
                                                                                        return;
                                                                                    }
                                                                                    IsStartEnabled = false;
                                                                                    SelectedWorkFlewItem.RunWorkFlews((UIElement)e.Source, true);
                                                                                });
        }

        [XmlIgnore]
        public RelayCommand<RoutedEventArgs> StartandContinueWorkFlewCommand { private set; get; }

        private void InitStartWorkFlewCommand()
        {
            StartWorkFlewCommand = new RelayCommand<RoutedEventArgs>(e =>
                                                                     {
                                                                         if (SelectedWorkFlewItem == null)
                                                                         {
                                                                             return;
                                                                         }
                                                                         IsStartEnabled = false;
                                                                         SelectedWorkFlewItem.RunWorkFlews((UIElement)e.Source, false);
                                                                     });
        }

        [XmlIgnore]
        public RelayCommand<RoutedEventArgs> StartWorkFlewCommand { private set; get; }

        private void InitWorkFlewStopCommand()
        {
            WorkFlewStopCommand = new RelayCommand<RoutedEventArgs>(e =>
                                                                    {
                                                                        var result = HasRunWorkFlew(WorkFlews);
                                                                        if (!result)
                                                                        {
                                                                            IsStartEnabled = true;
                                                                        }
                                                                    });
        }

        [XmlIgnore]
        public RelayCommand<RoutedEventArgs> WorkFlewStopCommand { private set; get; }

        private void InitStartTimeClickCommand()
        {
            StartTimeClickCommand = new RelayCommand(() =>
                                                     {
                                                         StopTime();
                                                         StartTimeText = DateTime.Now.ToString();
                                                         _timer.Start();
                                                     });
        }

        [XmlIgnore]
        public RelayCommand StartTimeClickCommand { private set; get; }

        private void InitStopTimeClickCommand()
        {
            StopTimeClickCommand = new RelayCommand(StopTime);
        }

        [XmlIgnore]
        public RelayCommand StopTimeClickCommand { private set; get; }

        private void InitCheckedPauseCommand()
        {
            CheckedPauseCommand = new RelayCommand(() => { _timer.Stop(); });
        }

        [XmlIgnore]
        public RelayCommand CheckedPauseCommand { private set; get; }

        private void InitUncheckedPauseCommand()
        {
            UncheckedPauseCommand = new RelayCommand(() => { _timer.Start(); });
        }

        [XmlIgnore]
        public RelayCommand UncheckedPauseCommand { private set; get; }
        #endregion
    }
}