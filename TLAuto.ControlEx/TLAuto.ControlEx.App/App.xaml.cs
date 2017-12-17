// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;

using TLAuto.BaseEx.Extensions;
using TLAuto.ControlEx.App.Common;
#endregion

namespace TLAuto.ControlEx.App
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : ApplicationEx
    {
        protected override bool IsCheckTwice => false;

        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigHelper.StartUpFilePath = e.Args.Length == 1 ? e.Args[0] : string.Empty;
            base.OnStartup(e);
        }
    }
}