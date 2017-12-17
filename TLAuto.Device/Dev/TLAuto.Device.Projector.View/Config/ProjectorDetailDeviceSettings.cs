// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Device.Projector.View.NavPages;
#endregion

namespace TLAuto.Device.Projector.View.Config
{
    public class ProjectorDetailDeviceSettings
    {
        public ProjectorDetailDeviceSettings() { }

        public ProjectorDetailDeviceSettings(ProjectorInfo projectorInfo)
        {
            ProjectorDeviceType = projectorInfo.ProjectorDeviceType;
            HeaderName = projectorInfo.ProjectorHeaderName;
            DeviceNumber = projectorInfo.DeviceNumber;
        }

        public ProjectorDeviceType ProjectorDeviceType { set; get; }

        public string HeaderName { set; get; }

        public int DeviceNumber { set; get; }

        public ProjectorInfo GetProjectorInfo()
        {
            return new ProjectorInfo
                   {
                       ProjectorDeviceType = ProjectorDeviceType,
                       ProjectorHeaderName = HeaderName,
                       DeviceNumber = DeviceNumber
                   };
        }
    }
}