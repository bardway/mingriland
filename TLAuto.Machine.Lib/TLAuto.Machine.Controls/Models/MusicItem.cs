// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Machine.Controls.Models
{
    public class MusicItem
    {
        public MusicItem(string serviceAddress)
        {
            ServiceAddress = serviceAddress;
        }

        public string ServiceAddress { get; }
    }
}