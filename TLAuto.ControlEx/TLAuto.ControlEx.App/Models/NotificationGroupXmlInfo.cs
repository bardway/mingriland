// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.ControlEx.App.Dialogs;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class NotificationGroupXmlInfo : ObservableObject
    {
        private bool _isExpanded = true;

        public NotificationGroupXmlInfo()
        {
            InitAddNotificationInfoCommand();
            InitRemovedNotificationInfoCommand();
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

        public ObservableCollection<MarkMatchInfo> MarkMatchInfos { get; } = new ObservableCollection<MarkMatchInfo>();

        public ObservableCollection<ServiceAddressInfo> AppNitificationServiceAddressInfos { get; } = new ObservableCollection<ServiceAddressInfo>();

        [XmlIgnore]
        public RelayCommand AddNotificationInfoCommand { private set; get; }

        [XmlIgnore]
        public RelayCommand RemovedNotificationInfoCommand { private set; get; }

        private void InitAddNotificationInfoCommand()
        {
            AddNotificationInfoCommand = new RelayCommand(() =>
                                                          {
                                                              var emd = new EditMarkServiceAddressDialog(string.Empty, string.Empty, AppNitificationServiceAddressInfos);
                                                              if (emd.ShowDialog() == true)
                                                              {
                                                                  if (MarkMatchInfos.FirstOrDefault(s => s.Mark == emd.Mark) == null)
                                                                  {
                                                                      MarkMatchInfos.Add(new MarkMatchInfo
                                                                                         {
                                                                                             Mark = emd.Mark,
                                                                                             ServiceAddressMark = emd.SelectedServiceAddressMark
                                                                                         });
                                                                  }
                                                              }
                                                          });
        }

        private void InitRemovedNotificationInfoCommand()
        {
            RemovedNotificationInfoCommand = new RelayCommand(() =>
                                                              {
                                                                  var removeInfos = MarkMatchInfos.Where(s => s.IsChecked).ToList();
                                                                  foreach (var removeInfo in removeInfos)
                                                                  {
                                                                      MarkMatchInfos.Remove(removeInfo);
                                                                  }
                                                              });
        }
    }
}