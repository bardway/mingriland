// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using TLAuto.Device.Controls.RichTextBoxEx;
using TLAuto.Device.DMX.View.Config;
using TLAuto.Log;
#endregion

namespace TLAuto.Device.DMX.View.Models
{
    public class DMXDeviceInfo : ViewModelBase
    {
        private const string LogModuleName = "PLC";
        private readonly LogWraper _logWraper = new LogWraper(LogModuleName);

        public DMXDeviceInfo(string headerName, int channelBegin, int channelEnd)
        {
            HeaderName = headerName;
            ChannelBegin = channelBegin;
            ChannelEnd = channelEnd;
            for (var channleNum = ChannelBegin; channleNum <= channelEnd; channleNum++)
            {
                ChannelInfos.Add(new DMXChannelInfo(channleNum, channelBegin));
            }
            InitOpenLogFileCommand();
        }

        public string HeaderName { get; }

        public int ChannelBegin { get; }

        public int ChannelEnd { get; }

        public ObservableCollection<DMXChannelInfo> ChannelInfos { get; } = new ObservableCollection<DMXChannelInfo>();

        public string LogMsgId { get; } = Guid.NewGuid().ToString();

        public override void Cleanup()
        {
            var deviceSettings = DMXDeviceSettings.GetDMXDeviceSettings();
            deviceSettings.DeviceInfosSettings.RemoveAll(s => s.HeaderName == HeaderName);
            base.Cleanup();
        }

        public void UpdateChannelRemarks(int channelNum, string remarks)
        {
            var channelInfoSettings = DMXDeviceSettings.GetDMXChannelInfoSettings(ChannelBegin, channelNum);
            channelInfoSettings.Remarks = remarks;
            DMXDeviceService.SaveSettings();
        }

        internal void WriteMsgWithLog(string msg, StatusNotificationType statusType)
        {
            Messenger.Default.Send(new StatusNotificationMessage(msg, statusType), LogMsgId);
            var logMsg = $"舞台灯光_{HeaderName}_{msg}";
            switch (statusType)
            {
                case StatusNotificationType.RInfo:
                    _logWraper.Info(logMsg);
                    break;
                case StatusNotificationType.NInfo:
                    _logWraper.Info(logMsg);
                    break;
                case StatusNotificationType.Error:
                    _logWraper.Error(logMsg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statusType), statusType, null);
            }
        }

        #region Event Mvvmbindings
        private void InitOpenLogFileCommand()
        {
            OpenLogFileCommand = new RelayCommand(() =>
                                                  {
                                                      var logFilePath = Path.Combine(Logger.RootLogPath, LogModuleName + ".log");
                                                      try
                                                      {
                                                          Process.Start(logFilePath);
                                                      }
                                                      catch (Exception ex)
                                                      {
                                                          MessageBox.Show(ex.Message);
                                                      }
                                                  });
        }

        public RelayCommand OpenLogFileCommand { private set; get; }
        #endregion
    }
}