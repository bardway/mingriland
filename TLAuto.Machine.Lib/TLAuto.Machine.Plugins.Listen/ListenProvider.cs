// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models;
using TLAuto.Machine.Plugins.Core.Models.Enums;
#endregion

namespace TLAuto.Machine.Plugins.Listen
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ListenProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Listen";
        private MachineButtonItem _listenButton;
        private MachineRelayItem _relayButton;

        private void Init()
        {
            _listenButton = ButtonItems[0];
            _relayButton = RelayItems[0];
        }

        public override void StartGame(DifficultyLevelType diffLevelType, string[] args)
        {
            Init();
            Task.Factory.StartNew(GameLogic);
        }

        private async void GameLogic()
        {
            await _relayButton.Control(true);
            while (true)
            {
                var result = await SendWcfCommandPluginsHelper.InvokerQueryDiaitalSwitchWithAutoUpload(_listenButton, 10000);
                if (result)
                {
                    await PlayMusic0(SignKey, "success.wav");
                    await Task.Delay(9000);
                }
            }
        }
    }
}