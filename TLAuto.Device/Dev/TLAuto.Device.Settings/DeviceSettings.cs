/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TLAuto.Base.Extensions;

namespace TLAuto.Device.Settings
{
    public class DeviceSettings
    {
        public void Save()
        {
            this.ToXmlFile(FilePath);
        }

        public bool Delete(out Exception exception)
        {
            exception = null;
            try
            {
                File.Delete(FilePath);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            return true;
        }

        [XmlIgnore]
        public string ServiceKey { internal set; get; }

        [XmlIgnore]
        internal string FilePath { set; private get; }

        public string HeaderName { set; get; }
    }
}