// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TLAuto.Log;
using TLAuto.Music.Contracts;
using TLAuto.Music.ServerHost.IocService;
using TLAuto.Music.ServerHost.ViewModels;
#endregion

namespace TLAuto.Music.ServerHost
{
    public class TLMusicService : ITLMusic
    {
        private static readonly ConcurrentBag<string> MusicMarks = new ConcurrentBag<string>();
        private static readonly LogWraper Logger = new LogWraper("TLMusicService");

        public WcfMusicSourceService MusicSourceService => (WcfMusicSourceService)ViewModelLocator.Instance.MusicSourceService;

        public async Task<bool> TestConnected()
        {
            return await MusicSourceService.TestConnected();
        }

        public async Task<bool> PlayMusic(string mark, string filePath, double volume, bool isRepeat)
        {
            if (MusicMarks.FirstOrDefault(s => s == mark) == null)
            {
                MusicMarks.Add(mark);
            }
            return await MusicSourceService.PlayMusic(mark, filePath, volume, isRepeat);
        }

        public async Task<bool> PauseMusic(string mark)
        {
            if (MusicMarks.FirstOrDefault(s => s == mark) != null)
            {
                return await MusicSourceService.PauseMusic(mark);
            }
            Logger.Error("并没有找到相关Mark。");
            return false;
        }

        public async Task<bool> PauseAllMusic()
        {
            if (MusicMarks.Count == 0)
            {
                Logger.Error("并没有找到相关Mark。");
                return false;
            }
            var failedResult = false;
            foreach (var mark in MusicMarks)
            {
                var currentResult = await MusicSourceService.PauseMusic(mark);
                if (!currentResult)
                {
                    failedResult = true;
                }
            }
            return !failedResult;
        }

        public async Task<bool> AdjustVolumeMusic(string mark, double volume)
        {
            if (MusicMarks.FirstOrDefault(s => s == mark) != null)
            {
                return await MusicSourceService.AdjustVolumeMusic(mark, volume);
            }
            Logger.Error("并没有找到相关Mark。");
            return false;
        }

        public async Task<bool> AdjustAllVolumeMusic(double volume)
        {
            if (MusicMarks.Count == 0)
            {
                Logger.Error("并没有找到相关Mark。");
                return false;
            }
            var failedResult = false;
            foreach (var mark in MusicMarks)
            {
                var currentResult = await MusicSourceService.AdjustVolumeMusic(mark, volume);
                if (!currentResult)
                {
                    failedResult = true;
                }
            }
            return !failedResult;
        }

        public async Task<IEnumerable<string>> GetMusicMarks()
        {
            return await MusicSourceService.GetMusicMarks();
        }

        public async Task<bool> SpeakFromText(string text, int volume)
        {
            return await MusicSourceService.SpeakFromText(text, volume);
        }
    }
}