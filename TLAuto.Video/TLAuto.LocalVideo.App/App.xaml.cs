// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.IO;
using System.Windows;

using TLAuto.Base.Extensions;
using TLAuto.LocalVideo.App.Common;
using TLAuto.Log;
#endregion

namespace TLAuto.LocalVideo.App
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private readonly LogWraper _log = new LogWraper("Video");

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                var videoPath = e.Args[0];
                var argsIndex = 1;
                while (!Path.HasExtension(videoPath))
                {
                    if (e.Args.Length >= argsIndex)
                    {
                        videoPath += " " + e.Args[argsIndex++];
                    }
                    else
                    {
                        break;
                    }
                }
                StartupArgsHelper.VideoFilePath = videoPath;
                if (e.Args.Length >= argsIndex)
                {
                    StartupArgsHelper.IsRepeat = e.Args[argsIndex++].ToBoolean();
                }
                _log.Info(StartupArgsHelper.VideoFilePath);
                _log.Info(StartupArgsHelper.IsRepeat ? "循环" : "一次");
            }
            base.OnStartup(e);
        }
    }
}