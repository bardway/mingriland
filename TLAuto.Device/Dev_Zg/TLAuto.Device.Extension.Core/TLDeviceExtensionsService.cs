// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Device.Contracts;
using TLAuto.Device.Extension.Core.DeviceExtensions.Config;
#endregion

namespace TLAuto.Device.Extension.Core
{
    public class TLDeviceExtensionsService
    {
        private const string ConfigPath = "DeviceExtensions/DeviceExtensionsConfig.xml";
        public static readonly TLDeviceExtensionsService Instance = new TLDeviceExtensionsService();
        private DeviceExtensionsConfig _config;
        private string _configPath;
        private bool _isLoaded;

        private TLDeviceExtensionsService() { }

        //public void RegistControlDeviceEx(string key, ControlInfo controlInfo, ITLAutoDevicePushCallback callBack)
        //{
        //    var deviceService = DeviceServices.FirstOrDefault(s => s.ServiceKey == controlInfo.ServiceKey);
        //    deviceService?.RegistControlDeviceEx(key, controlInfo.Data, callBack);
        //}

        //public void UnRegistControlDeviceEx(string key, string serviceKey)
        //{
        //    var deviceService = DeviceServices.FirstOrDefault(s => s.ServiceKey == serviceKey);
        //    deviceService?.UnRegistControlDeviceEx(key);
        //}

        [ImportMany]
        public IEnumerable<IDeviceService> DeviceServices { get; private set; }

        public bool LoadDeviceExtensions()
        {
            if (_isLoaded)
            {
                return false;
            }
            _isLoaded = true;
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            _configPath = Path.Combine(rootPath, ConfigPath);
            _config = _configPath.ToObjectFromXmlFile<DeviceExtensionsConfig>();
            var catalog = new AggregateCatalog();
            foreach (var deviceConfig in _config.DeviceConfigs)
            {
                catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(rootPath, deviceConfig.Path ?? string.Empty), deviceConfig.DllName + ".dll"));
            }
            var container = new CompositionContainer(catalog);
            try
            {
                container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                throw new ArgumentException(compositionException.Message);
            }
            return true;
        }

        public IDeviceService GetDeviceService(string serviceKey)
        {
            var deviceService = DeviceServices.FirstOrDefault(s => s.ServiceKey == serviceKey);
            if (deviceService != null)
            {
                return deviceService;
            }
            throw new ArgumentException("没有找到相关ServiceKey。");
        }

        public async Task<WcfResultInfo> ControlDevice(ControlInfo controlInfo)
        {
            var deviceService = DeviceServices.FirstOrDefault(s => s.ServiceKey == controlInfo.ServiceKey);
            if (deviceService != null)
            {
                return await deviceService.ControlDevice(controlInfo.Data);
            }
            return new WcfResultInfo {ErrorMsg = "没有找到对应的通信服务。"};
        }
    }
}