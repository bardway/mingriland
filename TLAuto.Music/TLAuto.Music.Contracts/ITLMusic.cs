// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
#endregion

namespace TLAuto.Music.Contracts
{
    [ServiceContract]
    public interface ITLMusic
    {
        [OperationContract]
        Task<bool> TestConnected();

        [OperationContract]
        Task<bool> PlayMusic(string mark, string filePath, double volume, bool isRepeat);

        [OperationContract]
        Task<bool> PauseMusic(string mark);

        [OperationContract]
        Task<bool> PauseAllMusic();

        [OperationContract]
        Task<bool> AdjustVolumeMusic(string mark, double volume);

        [OperationContract]
        Task<bool> AdjustAllVolumeMusic(double volume);

        [OperationContract]
        Task<IEnumerable<string>> GetMusicMarks();

        [OperationContract]
        Task<bool> SpeakFromText(string text, int volume);
    }
}