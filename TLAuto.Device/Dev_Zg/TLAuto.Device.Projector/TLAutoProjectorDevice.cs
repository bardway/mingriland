// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.Projector
{
    public class TLAutoProjectorDevice
    {
        private readonly string _logModuleName;
        private readonly int _timeOutMs;
        private readonly TLAutoDeviceSerialPort _tlAutoDeviceSerialPort;

        public TLAutoProjectorDevice
        (
            TLAutoDeviceSerialPort tlAutoDeviceSerialPort,
            ProjectorDeviceType projectorDeviceType,
            string logModuleName = null,
            int timeOutMs = 5000)
        {
            _tlAutoDeviceSerialPort = tlAutoDeviceSerialPort;
            _logModuleName = logModuleName;
            _timeOutMs = timeOutMs;
            InitProjectorDevice(projectorDeviceType);
        }

        public ITLAutoProjectorDeviceSerialPort TLAutoProjectorDeviceSerialPort { private set; get; }

        private void InitProjectorDevice(ProjectorDeviceType projectorDeviceType)
        {
            switch (projectorDeviceType)
            {
                case ProjectorDeviceType.Benq:
                    TLAutoProjectorDeviceSerialPort = new TLAutoMS524ProjectorDeviceSerialPort(_tlAutoDeviceSerialPort, _logModuleName, _timeOutMs);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(projectorDeviceType), projectorDeviceType, null);
            }
        }
    }
}