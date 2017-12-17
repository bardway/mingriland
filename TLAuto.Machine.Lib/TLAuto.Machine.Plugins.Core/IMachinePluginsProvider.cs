// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;

using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.Core.Models.Events;
#endregion

namespace TLAuto.Machine.Plugins.Core
{
    public interface IMachinePluginsProvider : IDisposable
    {
        object View { get; }

        void InitDeviceParam
        (
            List<MachineButtonItem> buttonItems,
            List<MachineRelayItem> relayItems,
            List<MachineMusicItem> musicItems);

        void StartGame(DifficultyLevelType diffLevelType, string[] args);

        event EventHandler<NotificationEventArgs> Notification;

        event EventHandler GameOver;
    }
}