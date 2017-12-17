// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Threading.Tasks;

using TLAuto.Machine.Plugins.Core.Models;
#endregion

namespace TLAuto.Machine.Plugins.Battle
{
    public sealed class HitButon
    {
        /// <summary>初始化 <see cref="T:System.Object" /> 类的新实例。</summary>
        public HitButon(int buttonId, MachineButtonItem hitter, MachineRelayItem light)
        {
            ButtonId = buttonId;
            Hitter = hitter;
            Light = light;
        }

        public int ButtonId { get; }

        public MachineButtonItem Hitter { get; }

        public MachineRelayItem Light { get; }

        public string Sound { get; set; }

        public bool? Hiited { get; private set; }

        public async Task Hit()
        {
            var lightoff = await Light.Control(false);
            if (lightoff)
            {
                Hiited = true;
            }
        }

        public async Task<bool> LightOn()
        {
            var lighton = await Light.Control(true);
            if (lighton)
            {
                Hiited = false;
            }
            return lighton;
        }

        public void ResetHitting()
        {
            Hiited = null;
        }
    }
}