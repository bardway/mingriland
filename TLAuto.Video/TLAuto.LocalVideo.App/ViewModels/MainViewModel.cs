// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using GalaSoft.MvvmLight;

using TLAuto.Video.Controls.AttachedPropertys;
#endregion

namespace TLAuto.LocalVideo.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private VideoWindowType _videoWindowType = VideoWindowType.FullScreen;

        public VideoWindowType VideoWindowType
        {
            set
            {
                _videoWindowType = value;
                RaisePropertyChanged();
            }
            get => _videoWindowType;
        }
    }
}