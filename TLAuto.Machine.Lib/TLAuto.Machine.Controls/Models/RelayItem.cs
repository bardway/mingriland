// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel;
using System.Runtime.CompilerServices;

using TLAuto.Machine.Controls.Annotations;
#endregion

namespace TLAuto.Machine.Controls.Models
{
    public class RelayItem : INotifyPropertyChanged
    {
        private bool _isOpen;

        public RelayItem(int deviceNumber, int number, string serviceAddress, string signName, int index)
        {
            DeviceNumber = deviceNumber;
            Number = number;
            ServiceAddress = serviceAddress;
            SignName = signName;
            Index = index;
        }

        public int Index { get; }

        public int DeviceNumber { get; }

        public int Number { get; }

        public string ServiceAddress { get; }

        public string SignName { get; }

        public bool IsOpen
        {
            set
            {
                _isOpen = value;
                OnPropertyChanged();
            }
            get => _isOpen;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}