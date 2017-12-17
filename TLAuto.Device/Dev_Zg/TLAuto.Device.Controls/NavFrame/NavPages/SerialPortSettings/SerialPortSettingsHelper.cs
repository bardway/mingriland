// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings
{
    public static class SerialPortSettingsHelper
    {
        private static IList<string> GetSerialPortNames()
        {
            return SerialPort.GetPortNames().ToList();
        }

        public static IEnumerable<string> GetSerialPortNames(IEnumerable<string> withoutPortNames)
        {
            var portNames = GetSerialPortNames();
            if (withoutPortNames != null)
            {
                foreach (var withoutPortName in withoutPortNames)
                {
                    portNames.Remove(withoutPortName);
                }
            }
            return portNames;
        }

        public static IEnumerable<int> GetSerialPortBaudRates()
        {
            return (from object value in Enum.GetValues(typeof(SerialPortBaudRates))
                    select value.ToInt32()).ToList();
        }

        public static IEnumerable<int> GetSerialPortDataBits()
        {
            return (from object value in Enum.GetValues(typeof(SerialPortDatabits))
                    select value.ToInt32()).ToList();
        }

        public static IEnumerable<int> GetParitys()
        {
            return (from object value in Enum.GetValues(typeof(Parity))
                    select value.ToInt32()).ToList();
        }

        public static IEnumerable<int> GetStopBits()
        {
            return (from object value in Enum.GetValues(typeof(StopBits))
                    select value.ToInt32()).ToList();
        }
    }
}