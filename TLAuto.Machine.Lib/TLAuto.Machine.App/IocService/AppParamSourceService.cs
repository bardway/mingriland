// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.IO;

using TLAuto.Base.Extensions;
using TLAuto.Notification.Contracts;
#endregion

namespace TLAuto.Machine.App.IocService
{
    public sealed class AppParamSourceService : IAppParamSourceService
    {
        private readonly AppParamInfo _appParamInfo;

        public AppParamSourceService()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppParamInfo.xml");
            _appParamInfo = File.Exists(filePath) ? filePath.ToObjectFromXmlFile<AppParamInfo>() : new AppParamInfo();
        }

        public IEnumerable<AppBoardParamInfo> GetAppInputBoardParamInfos()
        {
            return _appParamInfo.InputBoardParamInfos;
        }

        public IEnumerable<AppBoardParamInfo> GetAppOutputBoardParamInfos()
        {
            return _appParamInfo.OutputBoardParamInfos;
        }

        public IEnumerable<string> GetAppMusicParamInfos()
        {
            return _appParamInfo.MusicServiceAddressList;
        }
    }
}