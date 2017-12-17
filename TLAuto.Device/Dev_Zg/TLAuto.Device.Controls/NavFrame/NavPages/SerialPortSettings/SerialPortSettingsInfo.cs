// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Linq;

using TLAuto.BaseEx.Extensions;
#endregion

namespace TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings
{
    public class SerialPortSettingsInfo : ISettingsParam
    {
        private Tuple<SerialPortInfo, bool> _current;

        private IEnumerable<Tuple<SerialPortInfo, bool>> _usedSerialPortInfos;

        public SerialPortSettingsInfo(IList<Tuple<SerialPortInfo, bool>> usedSerialPortInfos = null, bool isEdit = false, string editPortName = null)
        {
            UsedSerialPortInfos = usedSerialPortInfos;
            IsEdit = isEdit;
            if (IsEdit && (usedSerialPortInfos != null))
            {
                var portInfo = usedSerialPortInfos.FirstOrDefault(s => s.Item1.PortName == editPortName);
                if (portInfo != null)
                {
                    Current = portInfo;
                }
            }
        }

        public IEnumerable<Tuple<SerialPortInfo, bool>> UsedSerialPortInfos { set => _usedSerialPortInfos = value; get => _usedSerialPortInfos ?? new List<Tuple<SerialPortInfo, bool>>(); }

        public Tuple<SerialPortInfo, bool> Current { private set => _current = value; get => _current ?? (_current = new Tuple<SerialPortInfo, bool>(new SerialPortInfo(), false)); }

        internal bool IsEdit { get; }

        public Uri PageUri { get; } = new Uri(UIExtensions.GetXamlUrl(typeof(SerialPortSettingsPage)), UriKind.Relative);

        public string HeaderName { get; } = "串口配置";
    }
}