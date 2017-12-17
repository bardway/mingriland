// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;

using TLAuto.Notification.Contracts;
#endregion

namespace TLAuto.Machine.App.IocService
{
    public interface IAppParamSourceService
    {
        IEnumerable<AppBoardParamInfo> GetAppInputBoardParamInfos();

        IEnumerable<AppBoardParamInfo> GetAppOutputBoardParamInfos();

        IEnumerable<string> GetAppMusicParamInfos();
    }
}