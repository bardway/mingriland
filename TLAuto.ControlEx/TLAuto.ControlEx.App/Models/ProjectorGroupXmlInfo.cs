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
    public class ProjectorGroupXmlInfo : ObservableObject
    {
        private bool _isExpanded = true;

        public ProjectorGroupXmlInfo()
        {
            InitAddProjectorInfoCommand();
            InitRemovedProjectorInfoCommand();
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

        public ObservableCollection<ProjectorMarkMatchInfo> ProjectorMarkMatchInfos { get; } = new ObservableCollection<ProjectorMarkMatchInfo>();

        public ObservableCollection<ServiceAddressInfo> ProjectorServiceAddressInfos { get; } = new ObservableCollection<ServiceAddressInfo>();

        [XmlIgnore]
        public RelayCommand AddProjectorInfoCommand { private set; get; }

        [XmlIgnore]
        public RelayCommand RemovedProjectorInfoCommand { private set; get; }

        private void InitAddProjectorInfoCommand()
        {
            AddProjectorInfoCommand = new RelayCommand(() =>
                                                       {
                                                           var emd = new EditMarkServiceAddressDialog(string.Empty, string.Empty, ProjectorServiceAddressInfos);
                                                           if (emd.ShowDialog() == true)
                                                           {
                                                               if (ProjectorMarkMatchInfos.FirstOrDefault(s => s.Mark == emd.Mark) == null)
                                                               {
                                                                   ProjectorMarkMatchInfos.Add(new ProjectorMarkMatchInfo
                                                                                               {
                                                                                                   Mark = emd.Mark,
                                                                                                   ServiceAddressMark = emd.SelectedServiceAddressMark
                                                                                               });
                                                               }
                                                           }
                                                       });
        }

        private void InitRemovedProjectorInfoCommand()
        {
            RemovedProjectorInfoCommand = new RelayCommand(() =>
                                                           {
                                                               var removeInfos = ProjectorMarkMatchInfos.Where(s => s.IsChecked).ToList();
                                                               foreach (var removeInfo in removeInfos)
                                                               {
                                                                   ProjectorMarkMatchInfos.Remove(removeInfo);
                                                               }
                                                           });
        }
    }
}