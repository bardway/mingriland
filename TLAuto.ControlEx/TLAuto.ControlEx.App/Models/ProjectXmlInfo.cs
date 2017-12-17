// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Serialization;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

using TLAuto.ControlEx.App.Models.Enums;
using TLAuto.ControlEx.App.Models.TreeModels;
using TLAuto.Log;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class ProjectXmlInfo : TreeItemXmlBase
    {
        private DMXGroupXmlInfo _dmxGroup;

        private BoardGroupXmlInfo _inputBoardGroup;
        private readonly string _logModuleName = "WorkFlewsLog";

        private MusicGroupXmlInfo _musicGroup;

        private NotificationGroupXmlInfo _notificationGroup;

        private BoardGroupXmlInfo _outputBoardGroup;

        private ProjectorGroupXmlInfo _projectorGroup;

        private WorkFlewGroupXmlInfo _restoreWorkFlewGroup;

        private DifficulySystemType _selectedDiffType = DifficulySystemType.Low;

        private int _textLogLineCount;

        private WorkFlewGroupXmlInfo _workFlewGroup;

        public ProjectXmlInfo()
        {
            InitOpenLogFileCommand();
            TxtInfo.Document.LineHeight = 3;
            Messenger.Default.Register<NotificationMessage>(this,
                                                            s => DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                                                                       {
                                                                                                           var status = MessageBoxResult.OK;
                                                                                                           if (s.Sender is MessageBoxResult)
                                                                                                           {
                                                                                                               status = (MessageBoxResult)s.Sender;
                                                                                                           }
                                                                                                           var paragraph = new Paragraph();
                                                                                                           paragraph.Inlines.Add(new Run(DateTime.Now + ": " + s.Notification) {Foreground = status == MessageBoxResult.OK ? Brushes.Black : Brushes.Red});
                                                                                                           TxtInfo.Document.Blocks.Add(paragraph);
                                                                                                           TxtInfo.ScrollToEnd();

                                                                                                           TextLogLineCount++;

                                                                                                           SaveLog(s.Notification + (status == MessageBoxResult.OK ? "" : "[错误，红色]"));
                                                                                                           if (TextLogLineCount == 1000)
                                                                                                           {
                                                                                                               TextLogLineCount = 0;
                                                                                                               TxtInfo.Document.Blocks.Clear();
                                                                                                           }
                                                                                                       }));
        }

        public ObservableCollection<ServiceAddressInfo> BoardServiceAddressInfos { get; } = new ObservableCollection<ServiceAddressInfo>();

        public BoardGroupXmlInfo InputBoardGroup { set => _inputBoardGroup = value; get => _inputBoardGroup ?? (_inputBoardGroup = new BoardGroupXmlInfo {BoardType = BoardType.InputA}); }

        public BoardGroupXmlInfo OutputBoardGroup { set => _outputBoardGroup = value; get => _outputBoardGroup ?? (_outputBoardGroup = new BoardGroupXmlInfo {BoardType = BoardType.OutputA}); }

        public MusicGroupXmlInfo MusicGroup { set => _musicGroup = value; get => _musicGroup ?? (_musicGroup = new MusicGroupXmlInfo()); }

        public NotificationGroupXmlInfo NotificationGroup { set => _notificationGroup = value; get => _notificationGroup ?? (_notificationGroup = new NotificationGroupXmlInfo()); }

        public DMXGroupXmlInfo DMXGroup { set => _dmxGroup = value; get => _dmxGroup ?? (_dmxGroup = new DMXGroupXmlInfo()); }

        public ProjectorGroupXmlInfo ProjectorGroup { set => _projectorGroup = value; get => _projectorGroup ?? (_projectorGroup = new ProjectorGroupXmlInfo()); }

        [XmlIgnore]
        public List<DifficulySystemType> DifficultyTypes
        {
            get
            {
                var names = Enum.GetNames(typeof(DifficulySystemType)).Select(s => (DifficulySystemType)Enum.Parse(typeof(DifficulySystemType), s)).ToList();
                return names;
            }
        }

        [XmlIgnore]
        public DifficulySystemType SelectedDiffType
        {
            set
            {
                _selectedDiffType = value;
                RaisePropertyChanged();
            }
            get => _selectedDiffType;
        }

        [XmlElement("WorkFlewGroup")]
        public WorkFlewGroupXmlInfo WorkFlewGroup { set => _workFlewGroup = value; get => _workFlewGroup ?? (_workFlewGroup = new WorkFlewGroupXmlInfo {WorkFlewGroupType = WorkFlewGroupType.Work}); }

        [XmlElement("RestoreWorkFlewGroup")]
        public WorkFlewGroupXmlInfo RestoreWorkFlewGroup { set => _restoreWorkFlewGroup = value; get => _restoreWorkFlewGroup ?? (_restoreWorkFlewGroup = new WorkFlewGroupXmlInfo {WorkFlewGroupType = WorkFlewGroupType.Restore}); }

        [XmlIgnore]
        public RichTextBox TxtInfo { get; } = new RichTextBox {IsReadOnly = true, VerticalScrollBarVisibility = ScrollBarVisibility.Auto};

        [XmlIgnore]
        public int TextLogLineCount
        {
            set
            {
                _textLogLineCount = value;
                RaisePropertyChanged();
            }
            get => _textLogLineCount;
        }

        private void SaveLog(string msg)
        {
            Logger.Info(_logModuleName, msg);
        }

        #region Event MvvmBindings
        private void InitOpenLogFileCommand()
        {
            OpenLogFileCommand = new RelayCommand(() =>
                                                  {
                                                      try
                                                      {
                                                          var projectName = Assembly.GetExecutingAssembly().GetName().Name;
                                                          var logSystemPath = @"TLLog\" + projectName;
                                                          Process.Start(Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), logSystemPath, _logModuleName + ".Log"));
                                                      }
                                                      catch (Exception ex)
                                                      {
                                                          MessageBox.Show(ex.Message);
                                                      }
                                                  });
        }

        [XmlIgnore]
        public RelayCommand OpenLogFileCommand { private set; get; }
        #endregion
    }
}