// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Linq;

using TLAuto.Device.Extension.Core;
#endregion

namespace TLAuto.Device.DMX.View.Config
{
    public class DMXDeviceSettings : DeviceSettings
    {
        public List<DMXDeviceInfoSettings> DeviceInfosSettings { get; } = new List<DMXDeviceInfoSettings>();

        public static DMXDeviceSettings GetDMXDeviceSettings()
        {
            return (DMXDeviceSettings)DMXDeviceService.GetDMXDeviceService().DeviceSettings;
        }

        public static DMXDeviceInfoSettings GetDMXDeviceInfoSettings(int channelBegin)
        {
            var dmxDeviceSettings = GetDMXDeviceSettings();
            var dmxDeviceInfoSettings = dmxDeviceSettings.DeviceInfosSettings.Find(s => s.ChannelBegin == channelBegin);
            return dmxDeviceInfoSettings;
        }

        public static DMXChannelInfoSettings GetDMXChannelInfoSettings(int channelBegin, int channelNum)
        {
            var dmxDeviceInfoSettings = GetDMXDeviceInfoSettings(channelBegin);
            return dmxDeviceInfoSettings.ChannelInfosSettings.FirstOrDefault(dmxChannelInfoSettingse => dmxChannelInfoSettingse.ChannelNum == channelNum);
        }
    }
}