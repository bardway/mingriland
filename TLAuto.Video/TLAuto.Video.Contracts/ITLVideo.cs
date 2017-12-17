// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ServiceModel;
using System.Threading.Tasks;
#endregion

namespace TLAuto.Video.Contracts
{
    [ServiceContract]
    public interface ITLVideo
    {
        [OperationContract]
        Task<bool> TestConnected();

        [OperationContract]
        Task<bool> PlayVideo(string videoPath, double volume, bool isRepeat);

        [OperationContract]
        Task<bool> InvokerAction(VideoActionType videoActionType);

        [OperationContract]
        Task<bool> SetPauseTimeEvent(TimeSpan time);

        [OperationContract]
        Task<bool> ChangeFrame(TimeSpan time, VideoActionType afterVideoActionType);
    }
}