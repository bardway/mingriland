// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using TLAuto.Device.Contracts;
#endregion

namespace TLAuto.Device.Extension.Core
{
    public abstract class DeviceService<T> : IDeviceService, INotifyPropertyChanged
    {
        private IDeviceSettings _deviceSettings;
        private bool _isInit;

        private IDisposable _view;

        public string WcfServiceAddress { private set; get; }

        public void Init(string wcfServiceAddress)
        {
            WcfServiceAddress = wcfServiceAddress;
            if (_isInit)
            {
                return;
            }
            _isInit = true;
            View = GetView();
        }

        public void Dispose()
        {
            if (View != null)
            {
                View.Dispose();
                _isInit = false;
                Exception ex;
                if (DeleteDeviceSettings(out ex))
                {
                    _deviceSettings = null;
                    View = null;
                }
            }
        }

        public abstract Task<WcfResultInfo> ControlDevice(byte[] data);

        public abstract void RegistControlDeviceEx(string key, byte[] data, ITLAutoDevicePushCallback callBack);

        public abstract void UnRegistControlDeviceEx(string key);

        public bool SaveDeviceSettings(out Exception exception)
        {
            return DeviceSettings.Save(typeof(T), out exception);
        }

        public bool DeleteDeviceSettings(out Exception exception)
        {
            return DeviceSettings.Delete(out exception);
        }

        public abstract string Description { get; }

        public abstract string ServiceKey { get; }

        public IDeviceSettings DeviceSettings => _deviceSettings ?? (_deviceSettings = DeviceSettingsHelper.LoadOrCreateDeviceSettings(ServiceKey, typeof(T)));

        public IDisposable View
        {
            private set
            {
                _view = value;
                OnPropertyChanged();
            }
            get => _view;
        }

        protected abstract IDisposable GetView();

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}