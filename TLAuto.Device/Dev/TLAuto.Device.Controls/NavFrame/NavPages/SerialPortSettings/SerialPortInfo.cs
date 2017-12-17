// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.IO.Ports;
#endregion

namespace TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings
{
    public class SerialPortInfo
    {
        public string PortSignName { set; get; }

        public string PortName { set; get; }

        public int BaudRates { set; get; }

        public int DataBits { set; get; }

        public Parity Parity { set; get; }

        public StopBits StopBits { set; get; }
    }
}