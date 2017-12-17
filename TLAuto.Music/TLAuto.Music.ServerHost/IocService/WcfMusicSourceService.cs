// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Threading.Tasks;

using TLAuto.Music.ServerHost.ViewModels;
#endregion

namespace TLAuto.Music.ServerHost.IocService
{
    public sealed class WcfMusicSourceService : IMusicSourceService
    {
        private MusicViewModel _musicVm;

        public async Task<bool> TestConnected()
        {
            return await Task.Factory.StartNew(() => true);
        }

        public void SetMusicUI(MusicViewModel musicVm)
        {
            _musicVm = musicVm;
        }

        public async Task<bool> PlayMusic(string mark, string filePath, double volume, bool isRepeat)
        {
            return await _musicVm.PlayMusic(mark, filePath, volume, isRepeat);
        }

        public async Task<bool> PauseMusic(string mark)
        {
            return await _musicVm.PauseMusic(mark);
        }

        public async Task<bool> AdjustVolumeMusic(string mark, double volume)
        {
            return await _musicVm.AdjustVolumeMusic(mark, volume);
        }

        public async Task<bool> SpeakFromText(string text, int volume)
        {
            return await _musicVm.SpeakFromText(text, volume);
        }

        public async Task<IEnumerable<string>> GetMusicMarks()
        {
            return await _musicVm.GetMusicMarks();
        }
    }
}