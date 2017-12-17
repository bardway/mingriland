// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Machine.Plugins.Core.Models
{
    public class MachineMusicItem
    {
        public MachineMusicItem(string serviceAddress)
        {
            ServiceAddress = serviceAddress;
        }

        public string ServiceAddress { get; }
    }
}