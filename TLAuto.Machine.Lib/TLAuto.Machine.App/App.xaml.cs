// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;

using TLAuto.Machine.App.Common;
using TLAuto.Machine.Plugins.Core;
#endregion

namespace TLAuto.Machine.App
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : ApplicationExForMachine
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MachineBuilder.Instance.Init(ConfigHelper.MachineKey);
            base.OnStartup(e);
        }
    }
}