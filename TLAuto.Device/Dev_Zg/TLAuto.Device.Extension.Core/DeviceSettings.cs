// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.IO;
using System.Xml.Serialization;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.Extension.Core
{
    public class DeviceSettings : IDeviceSettings
    {
        [XmlIgnore]
        internal string FilePath { set; private get; }

        public string HeaderName { set; get; }

        public bool Save(Type type, out Exception exception)
        {
            exception = null;
            try
            {
                this.ToXmlFile(type, FilePath);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            return true;
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
        public bool Exists { internal set; get; }
    }
}