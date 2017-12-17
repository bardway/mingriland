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
    public class MusicGroupXmlInfo : ObservableObject
    {
        private bool _isExpanded = true;

        public MusicGroupXmlInfo()
        {
            InitAddMusicInfoCommand();
            InitRemovedMusicInfoCommand();
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

        public ObservableCollection<MusicMarkMatchInfo> MusicMarkMatchInfos { get; } = new ObservableCollection<MusicMarkMatchInfo>();

        public ObservableCollection<ServiceAddressInfo> MusicServiceAddressInfos { get; } = new ObservableCollection<ServiceAddressInfo>();

        [XmlIgnore]
        public RelayCommand AddMusicInfoCommand { private set; get; }

        [XmlIgnore]
        public RelayCommand RemovedMusicInfoCommand { private set; get; }

        private void InitAddMusicInfoCommand()
        {
            AddMusicInfoCommand = new RelayCommand(() =>
                                                   {
                                                       var emd = new EditMarkServiceAddressDialog(string.Empty, string.Empty, MusicServiceAddressInfos);
                                                       if (emd.ShowDialog() == true)
                                                       {
                                                           if (MusicMarkMatchInfos.FirstOrDefault(s => s.Mark == emd.Mark) == null)
                                                           {
                                                               MusicMarkMatchInfos.Add(new MusicMarkMatchInfo
                                                                                       {
                                                                                           Mark = emd.Mark,
                                                                                           ServiceAddressMark = emd.SelectedServiceAddressMark
                                                                                       });
                                                           }
                                                       }
                                                   });
        }

        private void InitRemovedMusicInfoCommand()
        {
            RemovedMusicInfoCommand = new RelayCommand(() =>
                                                       {
                                                           var removeInfos = MusicMarkMatchInfos.Where(s => s.IsChecked).ToList();
                                                           foreach (var removeInfo in removeInfos)
                                                           {
                                                               MusicMarkMatchInfos.Remove(removeInfo);
                                                           }
                                                       });
        }
    }
}