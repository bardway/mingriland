// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Machine.Plugins.Core.Models
{
    public class MachineButtonItem
    {
        public MachineButtonItem(int deviceNumber, int number, string serviceAddress, string signName)
        {
            DeviceNumber = deviceNumber;
            Number = number;
            ServiceAddress = serviceAddress;
            SignName = signName;
        }

        public int DeviceNumber { get; }

        public int Number { get; }

        public string ServiceAddress { get; }

        public string SignName { get; }
    }
}