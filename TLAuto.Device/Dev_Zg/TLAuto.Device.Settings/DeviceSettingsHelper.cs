/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TLAuto.Base.Extensions;

namespace TLAuto.Device.Settings
{
    public static class DeviceSettingsHelper
    {
        private const string XmlDir = "Config";
        private static readonly string RootDir = AppDomain.CurrentDomain.BaseDirectory;
        private const string XmlName = "_DeviceSettings.xml";

        static DeviceSettingsHelper()
        {
            var dirPath = Path.Combine(RootDir, XmlDir);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            LoadAllSettingsFile(dirPath);
        }

        private static void LoadAllSettingsFile(string dirPath)
        {
            var files = Directory.GetFiles(dirPath, "*.xml");
            foreach (var file in files)
            {
                try
                {
                    var deviceSettings = file.ToObjectFromXmlFile<DeviceSettings>();
                    if (deviceSettings != null)
                    {
                        var fileName = Path.GetFileName(file);
                        if (fileName != null)
                        {
                            var settingsServiceKey = fileName.Replace(XmlName, "");
                            deviceSettings.ServiceKey = settingsServiceKey;
                            deviceSettings.FilePath = file;
                            if (DevicesSettings.Find(s => s.ServiceKey == deviceSettings.ServiceKey) == null)
                            {
                                DevicesSettings.Add(deviceSettings);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public static bool CreateDeviceSettings(string serviceKey, string headerName, out DeviceSettings deviceSettings)
        {
            deviceSettings = null;
            if (DevicesSettings.Find(s => s.ServiceKey == serviceKey) != null)
            {
                return false;
            }
            var fileName = serviceKey + XmlName;
            var filePath = Path.Combine(RootDir, XmlDir, fileName);
            deviceSettings = new DeviceSettings
            {
                ServiceKey = serviceKey,
                FilePath = filePath,
                HeaderName = headerName
            };
            deviceSettings.Save();
            DevicesSettings.Add(deviceSettings);
            return true;
        }

        public static List<DeviceSettings> DevicesSettings { get; } = new List<DeviceSettings>();
    }
}