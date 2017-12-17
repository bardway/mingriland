// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Base.Extensions;
using TLAuto.Device.Controls.RichTextBoxEx;
using TLAuto.Device.DMX.ServiceData;
#endregion

namespace TLAuto.Device.DMX.View.Models
{
    public class DMXChannelInfo : ViewModelBase
    {
        private readonly int _channelBegin;

        private double _channelValue;

        private string _remarks;

        public DMXChannelInfo(int channelNum, int channelBegin)
        {
            ChannelNum = channelNum;
            _channelBegin = channelBegin;
            InitUpdateRemarksCommand();
        }

        public int ChannelNum { get; }

        public string Remarks
        {
            set
            {
                _remarks = value;
                RaisePropertyChanged();
            }
            get => _remarks;
        }

        public double ChannelValue
        {
            set
            {
                _channelValue = value;
                RaisePropertyChanged();
                ControlChannel();
            }
            get => _channelValue;
        }

        private async void ControlChannel()
        {
            var channelValue = ChannelValue.ToInt32();
            var deviceInfo = DMXDeviceService.GetDMXDeviceInfo(_channelBegin);
            var resultInfo = await SendWcfCommandHelper.Send(new List<DMXControlServiceData>
                                                             {
                                                                 new DMXControlServiceData
                                                                 {
                                                                     ChannelNum = ChannelNum,
                                                                     ChannelValue = channelValue
                                                                 }
                                                             });
            if (resultInfo != null)
            {
                var msgInfo = resultInfo.IsError ? $"查询命令调用失败,原因：{resultInfo.ErrorMsg}" : "";
                if (!msgInfo.IsNullOrEmpty())
                {
                    deviceInfo.WriteMsgWithLog(msgInfo, StatusNotificationType.Error);
                }
                else
                {
                    if (resultInfo.Data == null)
                    {
                        deviceInfo.WriteMsgWithLog("调用命令超时或出现错误。", StatusNotificationType.Error);
                    }
                    else
                    {
                        var result = resultInfo.Data[0].ToBoolean();
                        if (!result)
                        {
                            deviceInfo.WriteMsgWithLog("调用命令超时或出现错误。", StatusNotificationType.Error);
                        }
                    }
                }
            }
            else
            {
                deviceInfo.WriteMsgWithLog(SendWcfCommandHelper.ErrorInfoForQueryFailed, StatusNotificationType.Error);
            }
        }

        #region Event Mvvmbindings
        private void InitUpdateRemarksCommand()
        {
            UpdateRemarksCommand = new RelayCommand(() =>
                                                    {
                                                        var deviceInfo = DMXDeviceService.GetDMXDeviceInfo(_channelBegin);
                                                        deviceInfo.UpdateChannelRemarks(ChannelNum, Remarks);
                                                    });
        }

        public RelayCommand UpdateRemarksCommand { private set; get; }
        #endregion
    }
}