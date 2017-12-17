// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
#endregion

namespace TLAuto.Device.PLC
{
    public class TLAutoPLCDevice
    {
        private readonly bool _hasControlRelayResultStatus;
        private readonly string _logModuleName;
        private readonly int _timeOutMs;
        private readonly TLAutoDeviceSerialPort _tlAutoDeviceSerialPort;

        public TLAutoPLCDevice
        (
            TLAutoDeviceSerialPort tlAutoDeviceSerialPort,
            bool hasControlRelayResultStatus,
            PLCDeviceType plcDeviceType,
            string logModuleName = null,
            int timeOutMs = 5000)
        {
            _tlAutoDeviceSerialPort = tlAutoDeviceSerialPort;
            _hasControlRelayResultStatus = hasControlRelayResultStatus;
            _logModuleName = logModuleName;
            _timeOutMs = timeOutMs;
            InitPLCDevice(plcDeviceType);
        }

        public ITLAutoPLCDeviceSerialPort TLAutoPLCDeviceSerialPort { private set; get; }

        private void InitPLCDevice(PLCDeviceType plcDeviceType)
        {
            switch (plcDeviceType)
            {
                case PLCDeviceType.Custom1:
                    TLAutoPLCDeviceSerialPort = new TLAutoCustom1PLCDeviceSerialPort(_tlAutoDeviceSerialPort, _hasControlRelayResultStatus, _logModuleName, _timeOutMs);
                    break;
                case PLCDeviceType.Custom2:
                    TLAutoPLCDeviceSerialPort = new TLAutoCustom2PLCDeviceSerialPort(_tlAutoDeviceSerialPort, _hasControlRelayResultStatus, _logModuleName, _timeOutMs);
                    break;
                case PLCDeviceType.Custom3:
                    TLAutoPLCDeviceSerialPort = new TLAutoCustom3PLCDeviceSerialPort(_tlAutoDeviceSerialPort, _hasControlRelayResultStatus, _logModuleName, _timeOutMs);
                    break;
                case PLCDeviceType.Custom4:
                    TLAutoPLCDeviceSerialPort = new TLAutoCustom4PLCDeviceSerialPort(_tlAutoDeviceSerialPort, _logModuleName, _timeOutMs);
                    break;
                case PLCDeviceType.ModbusRTU:
                    TLAutoPLCDeviceSerialPort = new TLAutoModbusRTUPLCDeviceSerialPort(_tlAutoDeviceSerialPort, _logModuleName, _timeOutMs);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(plcDeviceType), plcDeviceType, null);
            }
        }
    }
}