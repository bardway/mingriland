// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
#endregion

namespace TLAuto.Notification.Contracts
{
    public class AppParamInfo
    {
        public List<AppBoardParamInfo> InputBoardParamInfos { get; } = new List<AppBoardParamInfo>();

        public List<AppBoardParamInfo> OutputBoardParamInfos { get; } = new List<AppBoardParamInfo>();

        public List<string> MusicServiceAddressList { get; } = new List<string>();
    }
}