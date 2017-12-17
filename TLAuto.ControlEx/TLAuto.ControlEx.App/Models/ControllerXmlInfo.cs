// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

using GalaSoft.MvvmLight.Command;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Dialogs;
using TLAuto.ControlEx.App.Models.ControlleExcutes;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Board;
using TLAuto.ControlEx.App.Models.ControlleExcutes.DMX;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Music;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Notification;
using TLAuto.ControlEx.App.Models.Enums;
using TLAuto.ControlEx.App.Models.TreeModels;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class ControllerXmlInfo : TreeItemXmlBase
    {
        private string _cid;

        private string _description = "控制器描述";

        public ControllerXmlInfo()
        {
            InitAddExcuteCommand();
            InitRemoveExcuteCommand();
            InitUpExcuteCommand();
            InitDownExcuteCommand();
        }

        public string Description
        {
            set
            {
                _description = value;
                RaisePropertyChanged();
            }
            get => _description;
        }

        public string CID
        {
            set => _cid = value;
            get
            {
                if (string.IsNullOrEmpty(_cid))
                {
                    _cid = Guid.NewGuid().ToString();
                }
                return _cid;
            }
        }

        public ObservableCollection<ControllerExcute> Excutes { get; } = new ObservableCollection<ControllerExcute>();

        #region Event MvvmBindings
        private void InitAddExcuteCommand()
        {
            AddExcuteCommand = new RelayCommand(() =>
                                                {
                                                    var nce = new NewControllerExcuteDialog();
                                                    if (nce.ShowDialog() == true)
                                                    {
                                                        ControllerExcute controllerExcute;
                                                        switch (nce.SelectedExcuteType)
                                                        {
                                                            case ExcuteType.Switch:
                                                                controllerExcute = new SwitchBoardControllerExcute();
                                                                break;
                                                            case ExcuteType.Relay:
                                                                controllerExcute = new RelayBoardControllerExcute();
                                                                break;
                                                            case ExcuteType.Delay:
                                                                controllerExcute = new DelayControllerExcute();
                                                                break;
                                                            case ExcuteType.Music:
                                                                controllerExcute = new MusicControllerExcute();
                                                                break;
                                                            case ExcuteType.Projector:
                                                                controllerExcute = new ProjectorControllerExcute();
                                                                break;
                                                            case ExcuteType.Notification:
                                                                controllerExcute = new NotificationControllerExcute();
                                                                break;
                                                            case ExcuteType.DMX:
                                                                controllerExcute = new DMXControllerExcute();
                                                                break;
                                                            default:
                                                                throw new ArgumentOutOfRangeException();
                                                        }
                                                        Excutes.Add(controllerExcute);
                                                    }
                                                });
        }

        [XmlIgnore]
        public RelayCommand AddExcuteCommand { private set; get; }

        private void InitRemoveExcuteCommand()
        {
            RemoveExcuteCommand = new RelayCommand(() =>
                                                   {
                                                       var removeItems = Excutes.Where(s => s.IsChecked).ToList();
                                                       foreach (var item in removeItems)
                                                       {
                                                           Excutes.Remove(item);
                                                       }
                                                   });
        }

        [XmlIgnore]
        public RelayCommand RemoveExcuteCommand { private set; get; }

        private void InitUpExcuteCommand()
        {
            UpExcuteCommand = new RelayCommand(() =>
                                               {
                                                   var excute = Excutes.FirstOrDefault(s => s.IsChecked);
                                                   if (excute != null)
                                                   {
                                                       Excutes.Up(excute);
                                                   }
                                               });
        }

        [XmlIgnore]
        public RelayCommand UpExcuteCommand { private set; get; }

        private void InitDownExcuteCommand()
        {
            DownExcuteCommand = new RelayCommand(() =>
                                                 {
                                                     var excute = Excutes.FirstOrDefault(s => s.IsChecked);
                                                     if (excute != null)
                                                     {
                                                         Excutes.Down(excute);
                                                     }
                                                 });
        }

        [XmlIgnore]
        public RelayCommand DownExcuteCommand { private set; get; }
        #endregion
    }
}