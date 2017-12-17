// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml.Serialization;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Common;
using TLAuto.ControlEx.App.Models.Enums;
using TLAuto.Device.Contracts;
using TLAuto.Device.Projector.ServiceData;
#endregion

namespace TLAuto.ControlEx.App.Models.ControlleExcutes
{
    [Description("投影仪执行器")]
    public class ProjectorControllerExcute : ControllerExcute
    {
        private string _selectedProjectorMark;

        private ProjectorType _selectedProjectorType;

        public ProjectorControllerExcute()
        {
            var names = Enum.GetNames(typeof(ProjectorType));
            foreach (var name in names)
            {
                ProjectorTypes.Add((ProjectorType)Enum.Parse(typeof(ProjectorType), name));
            }
            SelectedProjectorType = ProjectorType.PowerOn;
        }

        [XmlIgnore]
        public ObservableCollection<ProjectorMarkMatchInfo> ProjectorMarks => ProjectHelper.Project.ItemXmlInfo.ProjectorGroup.ProjectorMarkMatchInfos;

        public string SelectedProjectorMark
        {
            set
            {
                _selectedProjectorMark = value;
                RaisePropertyChanged();
            }
            get => _selectedProjectorMark;
        }

        [XmlIgnore]
        public List<ProjectorType> ProjectorTypes { get; } = new List<ProjectorType>();

        public ProjectorType SelectedProjectorType
        {
            set
            {
                _selectedProjectorType = value;
                RaisePropertyChanged();
            }
            get => _selectedProjectorType;
        }

        public override async Task<bool> Excute(Action<string> writeLogMsgAction)
        {
            var serviceAddress = ProjectHelper.GetProjectorServiceAddress(SelectedProjectorMark);
            var portName = ProjectHelper.GetProjectorPortName(SelectedProjectorMark);
            var sendWcfCommand = new SendWcfCommand<ITLAutoDevice>(serviceAddress, writeLogMsgAction);
            switch (SelectedProjectorType)
            {
                case ProjectorType.PowerOn:
                    return await sendWcfCommand.SendAsync(async proxy =>
                                                          {
                                                              var controlInfo = new ControlInfo {ServiceKey = ConfigHelper.ProjectorServiceKey};
                                                              var serviceData = new ProjectorControlServiceData
                                                                                {
                                                                                    PortSignName = portName,
                                                                                    PowerOnOrOff = true,
                                                                                    DeviceNumber = 1
                                                                                };
                                                              controlInfo.Data = serviceData.ToBytes();
                                                              var result = await proxy.ControlDevice(controlInfo);
                                                              if (!result.IsError && (result.Data != null))
                                                              {
                                                                  return result.Data[0].ToBoolean();
                                                              }
                                                              return false;
                                                          });
                case ProjectorType.PowerOff:
                    return await sendWcfCommand.SendAsync(async proxy =>
                                                          {
                                                              var controlInfo = new ControlInfo {ServiceKey = ConfigHelper.ProjectorServiceKey};
                                                              var serviceData = new ProjectorControlServiceData
                                                                                {
                                                                                    PortSignName = portName,
                                                                                    PowerOnOrOff = false,
                                                                                    DeviceNumber = 1
                                                                                };
                                                              controlInfo.Data = serviceData.ToBytes();
                                                              var result = await proxy.ControlDevice(controlInfo);
                                                              if (!result.IsError && (result.Data != null))
                                                              {
                                                                  return result.Data[0].ToBoolean();
                                                              }
                                                              return false;
                                                          });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}