// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Threading.Tasks;

using TLAuto.Music.ServerHost.ViewModels;
#endregion

namespace TLAuto.Music.ServerHost.IocService
{
    public interface IMusicSourceService
    {
        Task<bool> TestConnected();

        void SetMusicUI(MusicViewModel musicVm);
    }
}