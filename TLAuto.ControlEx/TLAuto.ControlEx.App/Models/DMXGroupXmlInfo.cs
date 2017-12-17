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
    public class DMXGroupXmlInfo : ObservableObject
    {
        private bool _isExpanded = true;

        public DMXGroupXmlInfo()
        {
            InitAddDMXInfoCommand();
            InitRemovedDMXInfoCommand();
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

        public ObservableCollection<ServiceAddressInfo> DMXServiceAddressInfos { get; } = new ObservableCollection<ServiceAddressInfo>();

        [XmlIgnore]
        public RelayCommand AddDMXInfoCommand { private set; get; }

        [XmlIgnore]
        public RelayCommand RemovedDMXInfoCommand { private set; get; }

        private void InitAddDMXInfoCommand()
        {
            AddDMXInfoCommand = new RelayCommand(() =>
                                                 {
                                                     var emd = new EditMarkServiceAddressDialog(string.Empty, string.Empty, DMXServiceAddressInfos);
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

        private void InitRemovedDMXInfoCommand()
        {
            RemovedDMXInfoCommand = new RelayCommand(() =>
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