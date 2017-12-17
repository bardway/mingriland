// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows.Controls;
#endregion

namespace TLAuto.Device.Controls.NavFrame
{
    public abstract class NavPage : Page, INotifyNavStatusChanged
    {
        protected void UpdateNavStatus(bool navStatus)
        {
            OnNavStatusChanged(new NavStatusChangedEventArgs(navStatus));
            UpdateSettingsParam();
        }

        protected abstract void UpdateSettingsParam();

        #region INavContent
        public object ParamObj { set; get; }

        public event NavStatusChangedEventHandler NavStatusChanged;

        private void OnNavStatusChanged(NavStatusChangedEventArgs e)
        {
            NavStatusChanged?.Invoke(this, e);
        }
        #endregion
    }
}