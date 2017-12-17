// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Device.Controls.NavFrame
{
    public delegate void NavStatusChangedEventHandler(object sender, NavStatusChangedEventArgs e);

    public interface INotifyNavStatusChanged
    {
        object ParamObj { set; get; }

        event NavStatusChangedEventHandler NavStatusChanged;
    }
}