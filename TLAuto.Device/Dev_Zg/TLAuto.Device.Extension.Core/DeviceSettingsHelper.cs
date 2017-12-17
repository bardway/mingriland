// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.IO;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.Extension.Core
{
    public static class DeviceSettingsHelper
    {
        private const string XmlName = "_DeviceSettings.xml";
        private static readonly string RootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceExtensions");

        static DeviceSettingsHelper()
        {
            if (!Directory.Exists(RootDir))
            {
                Directory.CreateDirectory(RootDir);
            }
        }

        public static DeviceSettings LoadOrCreateDeviceSettings(string serviceKey, Type type)
        {
            var fileName = Path.Combine(RootDir, serviceKey + XmlName);
            DeviceSettings deviceSettings;
            var fileExists = File.Exists(fileName);
            if (fileExists)
            {
                deviceSettings = fileName.ToObjectFromXmlFile(type) as DeviceSettings;
            }
            else
            {
                deviceSettings = Activator.CreateInstance(type) as DeviceSettings;
            }
            if (deviceSettings != null)
            {
                deviceSettings.FilePath = fileName;
                deviceSettings.Exists = fileExists;
            }
            return deviceSettings;
        }
    }
}