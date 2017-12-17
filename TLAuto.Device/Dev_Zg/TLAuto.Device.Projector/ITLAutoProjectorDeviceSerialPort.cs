// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Threading.Tasks;
#endregion

namespace TLAuto.Device.Projector
{
    public interface ITLAutoProjectorDeviceSerialPort
    {
        Task<bool> PowerOn();

        Task<bool> PowerOff();
    }
}