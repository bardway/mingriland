// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using GalaSoft.MvvmLight.Command;

using TLAuto.Machine.Controls.Annotations;
#endregion

namespace TLAuto.Machine.Controls.Models
{
    public class SwitchItem : INotifyPropertyChanged
    {
        public SwitchItem(int deviceNumber, int number, string serviceAddress, string signName, int index)
        {
            DeviceNumber = deviceNumber;
            Number = number;
            ServiceAddress = serviceAddress;
            SignName = signName;
            Index = index;
            InitClickCommand();
        }

        public int Index { get; }

        public int DeviceNumber { get; }

        public int Number { get; }

        public string ServiceAddress { get; }

        public string SignName { get; }

        public RelayCommand ClickCommand { private set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InitClickCommand()
        {
            ClickCommand = new RelayCommand(OnClick);
        }

        public event EventHandler Click;

        protected virtual void OnClick()
        {
            Click?.Invoke(this, EventArgs.Empty);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}