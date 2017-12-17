// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.ControlEx.App.Models
{
    public class ProjectorMarkMatchInfo : MarkMatchInfo
    {
        private string _signName;

        public string SignName
        {
            set
            {
                _signName = value;
                RaisePropertyChanged();
            }
            get => _signName;
        }
    }
}