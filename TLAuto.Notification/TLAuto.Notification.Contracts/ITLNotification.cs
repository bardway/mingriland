// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
#endregion

namespace TLAuto.Notification.Contracts
{
    [ServiceContract]
    public interface ITLNotification
    {
        [OperationContract]
        Task<bool> TestConnected();

        [OperationContract]
        Task<bool> AddAppStatus(string appKey, AppStatusType appStatusType);

        [OperationContract]
        Task<bool> StartApp(string appKey, string filePath, string startArgs, IEnumerable<BoardNotificationInfo> inputBoardInfos, IEnumerable<BoardNotificationInfo> outputBoardInfos, IEnumerable<string> musicInfos);

        [OperationContract]
        Task<bool> StopApp(string processName);

        [OperationContract]
        Task<bool> ShutDown();

        [OperationContract]
        Task<AppStatusType> GetAppStatus(string appKey);
    }
}