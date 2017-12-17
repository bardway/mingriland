// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using GalaSoft.MvvmLight;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class ServiceAddressInfo : ObservableObject
    {
        private string _mark;

        private string _serviceAddress;

        public string Mark
        {
            set
            {
                _mark = value;
                RaisePropertyChanged();
            }
            get => _mark;
        }

        public string ServiceAddress
        {
            set
            {
                _serviceAddress = value;
                RaisePropertyChanged();
            }
            get => _serviceAddress;
        }
    }
}