// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Notification.Contracts;
#endregion

namespace TLAuto.Notification.ServerHost.Models
{
    public class NotificationInfo : ObservableObject
    {
        private AppStatusType _appStatusType;

        public NotificationInfo(string appKey)
        {
            AppKey = appKey;
            InitRemoveCommand();
        }

        public string AppKey { get; }

        public AppStatusType AppStatusType
        {
            set
            {
                _appStatusType = value;
                RaisePropertyChanged();
            }
            get => _appStatusType;
        }

        public RelayCommand RemoveCommand { private set; get; }

        private void InitRemoveCommand()
        {
            RemoveCommand = new RelayCommand(OnRemoved);
        }

        public event EventHandler Removed;

        protected virtual void OnRemoved()
        {
            var handler = Removed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}