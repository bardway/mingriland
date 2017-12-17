// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.ObjectModel;
using System.Xml.Serialization;

using GalaSoft.MvvmLight;

using TLAuto.ControlEx.App.Common;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes.Music
{
    public class MusicMarkManager : ObservableObject
    {
        private string _selectedMusicMark;

        [XmlIgnore]
        public ObservableCollection<MusicMarkMatchInfo> MusicMarks => ProjectHelper.Project.ItemXmlInfo.MusicGroup.MusicMarkMatchInfos;

        public string SelectedMusicMark
        {
            set
            {
                _selectedMusicMark = value;
                RaisePropertyChanged();
            }
            get => _selectedMusicMark;
        }
    }
}