// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
#endregion

namespace TLAuto.Device.DMX.View.Config
{
    public class DMXDeviceInfoSettings
    {
        public DMXDeviceInfoSettings() { }

        public DMXDeviceInfoSettings(string headerName, int channelBegin, int channelEnd)
        {
            HeaderName = headerName;
            ChannelBegin = channelBegin;
            ChannelEnd = channelEnd;
            for (var channelNum = ChannelBegin; channelNum <= ChannelEnd; channelNum++)
            {
                ChannelInfosSettings.Add(new DMXChannelInfoSettings
                                         {
                                             ChannelNum = channelNum
                                         });
            }
        }

        public string HeaderName { set; get; }

        public int ChannelBegin { set; get; }

        public int ChannelEnd { set; get; }

        public List<DMXChannelInfoSettings> ChannelInfosSettings { get; } = new List<DMXChannelInfoSettings>();
    }
}