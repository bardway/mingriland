// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.ComponentModel.Composition;

using TLAuto.Machine.Plugins.Core;
using TLAuto.Machine.Plugins.Core.Models.Enums;
using TLAuto.Machine.Plugins.Help.Views;
#endregion

namespace TLAuto.Machine.Plugins.Help
{
    [Export(SignKey, typeof(IMachinePluginsProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class HelpMachinePluginsProvider : MachinePluginsProvider<Main>
    {
        private const string SignKey = "Help";

        public override void StartGame(DifficultyLevelType diffLevelType, string[] args) { }
    }
}