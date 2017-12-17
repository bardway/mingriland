// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

using GalaSoft.MvvmLight.Command;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.Device.Contracts;
using TLAuto.Device.DMX.ServiceData;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.DMX
{
    [Description("舞台灯光执行器")]
    public class DMXControllerExcute : ControllerExcute
    {
        private string _dmxSign;

        public DMXControllerExcute()
        {
            InitAddDMXItemExcuteCommand();
            InitRemoveDMXItemExcuteCommand();
            InitDownDMXItemExcuteCommand();
            InitUpDMXItemExcuteCommand();
        }

        public string DMXSign
        {
            set
            {
                _dmxSign = value;
                RaisePropertyChanged();
            }
            get => _dmxSign;
        }

        public ObservableCollection<DMXChannelInfo> DMXChannelInfos { get; } = new ObservableCollection<DMXChannelInfo>();

        public override async Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            var serviceAddress = ProjectHelper.GetDMXServiceAddress(DMXSign);
            var wcfCommand = new SendWcfCommand<ITLAutoDevice>(serviceAddress, writeLogMsgAction);
            var result = await wcfCommand.SendAsync(async proxy =>
                                                    {
                                                        try
                                                        {
                                                            var controlInfo = new ControlInfo {ServiceKey = ConfigHelper.DMXServiceKey};
                                                            var datas = DMXChannelInfos.Select(s => new DMXControlServiceData {ChannelNum = s.ChannelNumber, ChannelValue = s.ChannelValue}).ToList();
                                                            controlInfo.Data = datas.ToBytes();
                                                            var resultInfo = await proxy.ControlDevice(controlInfo);
                                                            if (!resultInfo.IsError && (resultInfo.Data != null) && resultInfo.Data[0].ToBoolean())
                                                            {
                                                                return true;
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            writeLogMsgAction("执行任务时出错，原因为：" + ex.Message);
                                                        }
                                                        return false;
                                                    });
            return result;
        }

        #region Event MvvmBindings
        private void InitAddDMXItemExcuteCommand()
        {
            AddDMXItemExcuteCommand = new RelayCommand(() => { DMXChannelInfos.Add(new DMXChannelInfo()); });
        }

        [XmlIgnore]
        public RelayCommand AddDMXItemExcuteCommand { private set; get; }

        private void InitRemoveDMXItemExcuteCommand()
        {
            RemoveDMXItemExcuteCommand = new RelayCommand(() =>
                                                          {
                                                              var removeList = DMXChannelInfos.Where(s => s.IsChecked).ToList();
                                                              foreach (var item in removeList)
                                                              {
                                                                  DMXChannelInfos.Remove(item);
                                                              }
                                                          });
        }

        [XmlIgnore]
        public RelayCommand RemoveDMXItemExcuteCommand { private set; get; }

        private void InitUpDMXItemExcuteCommand()
        {
            UpDMXItemExcuteCommand = new RelayCommand(() =>
                                                      {
                                                          var dmxChannelInfo = DMXChannelInfos.FirstOrDefault(s => s.IsChecked);
                                                          if (dmxChannelInfo != null)
                                                          {
                                                              DMXChannelInfos.Up(dmxChannelInfo);
                                                          }
                                                      });
        }

        [XmlIgnore]
        public RelayCommand UpDMXItemExcuteCommand { private set; get; }

        private void InitDownDMXItemExcuteCommand()
        {
            DownDMXItemExcuteCommand = new RelayCommand(() =>
                                                        {
                                                            var dmxChannelInfo = DMXChannelInfos.FirstOrDefault(s => s.IsChecked);
                                                            if (dmxChannelInfo != null)
                                                            {
                                                                DMXChannelInfos.Down(dmxChannelInfo);
                                                            }
                                                        });
        }

        [XmlIgnore]
        public RelayCommand DownDMXItemExcuteCommand { private set; get; }
        #endregion
    }
}