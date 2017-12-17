// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Device.Models
{
    public abstract class SerialPortResultInfo<T>
    {
        public T Result { set; get; }

        public bool IsTimeout { set; get; }
    }
}