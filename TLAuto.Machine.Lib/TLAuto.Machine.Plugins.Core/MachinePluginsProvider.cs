// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.Core.Models.Events;
#endregion

namespace TLAuto.Machine.Plugins.Core
{
    public abstract class MachinePluginsProvider<T> : IMachinePluginsProvider
    {
        protected MachinePluginsProvider()
        {
            View = Activator.CreateInstance(typeof(T));
            SendWcfCommandPluginsHelper.WcfNotification += SendWcfCommandHelper_WcfNotification;
        }

        public List<MachineButtonItem> ButtonItems { private set; get; }

        public List<MachineRelayItem> RelayItems { private set; get; }

        public List<MachineMusicItem> MusicItems { private set; get; }

        public T MainUI => (T)View;

        public void InitDeviceParam(List<MachineButtonItem> buttonItems, List<MachineRelayItem> relayItems, List<MachineMusicItem> musicItems)
        {
            ButtonItems = buttonItems;
            RelayItems = relayItems;
            MusicItems = musicItems;
        }

        public abstract void StartGame(DifficultyLevelType diffLevelType, string[] args);

        public virtual void Dispose() { }

        public object View { get; }

        private void SendWcfCommandHelper_WcfNotification(object sender, NotificationEventArgs e)
        {
            OnNotification(new NotificationEventArgs(e.Notification));
        }

        public async Task PlayTextMusicFromFirstItem(string text)
        {
            await SendWcfCommandPluginsHelper.InvokerTextToMusic(text, MusicItems[0].ServiceAddress);
            OnNotification(new NotificationEventArgs(text));
        }

        public async Task<bool> PlayMusic0(string key, string fileName, string musicKey = null, double volume = 1, bool isRepeat = false, string musicAddress = null)
        {
            var musicPathBase = CommonConfigHelper.MusicBasePath;
            musicPathBase = @"C:\Program Files\StartGateServer\TLAuto.Machine\" + key + @"\MachinePlugins\Music";
            return await SendWcfCommandPluginsHelper.InvokerMusic(key + (musicKey.IsNullOrEmpty() ? "" : musicKey), Path.Combine(musicPathBase, fileName), volume, isRepeat, musicAddress ?? MusicItems[0].ServiceAddress);
        }

        public async Task<bool> PauseMusic0(string key, string musicKey = null, string musicAddress = null)
        {
            return await SendWcfCommandPluginsHelper.InvokerPauseMusic(key + (musicKey.IsNullOrEmpty() ? "" : musicKey), musicAddress ?? MusicItems[0].ServiceAddress);
        }

        protected virtual void CallOnNotification(string message)
        {
            OnNotification(new NotificationEventArgs($"{DateTime.Now} : {message}"));
        }

        //public async Task<bool> PlayVideo(string key, string fileName, bool isRepeat, string videoAddress)
        //{
        //    var videoBasePath = CommonConfigHelper.VideoBasePath;
        //    var filePathBase = @"C:\Program Files\StartGateServer\TLAuto.Machine\" + key + @"\MachinePlugins\Video";
        //    return await SendWcfCommandPluginsHelper.InvokerPlayVideo(key, videoBasePath, Path.Combine(filePathBase, fileName) + " " + isRepeat, videoAddress);
        //}

        //public async Task<bool> StopVideo(string videoAddress)
        //{
        //    return await SendWcfCommandPluginsHelper.InvokerStopVideo(videoAddress);
        //}

        #region Events
        public event EventHandler<NotificationEventArgs> Notification;

        protected virtual void OnNotification(NotificationEventArgs e)
        {
            Notification?.Invoke(this, e);
        }

        public event EventHandler GameOver;

        protected virtual void OnGameOver()
        {
            GameOver?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}