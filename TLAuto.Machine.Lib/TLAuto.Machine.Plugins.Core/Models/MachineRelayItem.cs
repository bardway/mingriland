// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Threading.Tasks;
#endregion

namespace TLAuto.Machine.Plugins.Core.Models
{
    public class MachineRelayItem
    {
        public MachineRelayItem(int deviceNumber, int number, string serviceAddress, string signName)
        {
            DeviceNumber = deviceNumber;
            Number = number;
            ServiceAddress = serviceAddress;
            SignName = signName;
        }

        public int DeviceNumber { get; }

        public int Number { get; }

        public string ServiceAddress { get; }

        public string SignName { get; }

        public bool IsNo { private set; get; }

        public async Task<bool> Control()
        {
            return await Control(!IsNo);
        }

        public async Task<bool> Control(bool isNo)
        {
            //var result = await Task.Factory.StartNew(() => true);
            var result = await SendWcfCommandPluginsHelper.InvokerControlRelay(this, isNo);
            if (result)
            {
                IsNo = isNo;
                OnRelayStatusChanged();
            }
            return result;
        }

        public event EventHandler RelayStatusChanged;

        protected virtual void OnRelayStatusChanged()
        {
            RelayStatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}