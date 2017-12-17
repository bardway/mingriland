// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;

using TLAuto.BaseEx.Extensions;
using TLAuto.Device.ServerHost.Config;
#endregion

namespace TLAuto.Device.ServerHost
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : ApplicationEx
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigHelper.Init();
            base.OnStartup(e);
        }
    }
}