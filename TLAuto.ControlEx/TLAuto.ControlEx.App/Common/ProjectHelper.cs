// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TLAuto.Base.Extensions;
using TLAuto.ControlEx.App.Models;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Board;
using TLAuto.ControlEx.App.Models.ControlleExcutes.Music;
using TLAuto.ControlEx.App.Models.Enums;
using TLAuto.ControlEx.App.Models.TreeModels;
#endregion

namespace TLAuto.ControlEx.App.Common
{
    public static class ProjectHelper
    {
        public static ProjectInfo Project { private set; get; }

        public static void InitProject(ProjectInfo projectInfo)
        {
            Project = projectInfo;
        }

        public static void CloseProject()
        {
            Project = null;
        }

        public static ProjectInfo GetProjectInfo(string fileName, string location)
        {
            var projectInfo = new ProjectInfo(fileName + ".tlcproj", location);
            TreeItemHelper.LoadProjectItems(projectInfo);
            return projectInfo;
        }

        public static void SaveAll(TreeItemBase itemBase)
        {
            Project.SaveAll(itemBase);
        }

        public static string GetDMXServiceAddress(string dmxSign)
        {
            var appServiceMark = Project.ItemXmlInfo.DMXGroup.MarkMatchInfos.FirstOrDefault(s => s.Mark == dmxSign);
            if (appServiceMark != null)
            {
                var serviceAddressMark = appServiceMark.ServiceAddressMark;
                var serviceAddressInfo = Project.ItemXmlInfo.DMXGroup.DMXServiceAddressInfos.FirstOrDefault(s => s.Mark == serviceAddressMark);
                if (serviceAddressInfo != null)
                {
                    return serviceAddressInfo.ServiceAddress;
                }
            }
            return string.Empty;
        }

        public static string GetBoardServiceAddress(int deviceNumber, BoardType boardType)
        {
            var serviceAddressList = Project.ItemXmlInfo.BoardServiceAddressInfos.ToList();
            var boardInfos = GetBoardInfos(boardType);
            foreach (var boardInfo in boardInfos)
            {
                if (boardInfo.DeviceNumber == deviceNumber)
                {
                    var service = serviceAddressList.FirstOrDefault(s => s.Mark == boardInfo.ServiceAddressMark);
                    if (service != null)
                    {
                        return service.ServiceAddress;
                    }
                }
            }
            return string.Empty;
        }

        public static List<MusicControllerExcute> GetMusicControllerExcutes()
        {
            var controllerInfos = new List<ControllerInfo>();
            GetAllControllerInfo(Project, ref controllerInfos);
            var musicControllerExcutes = new List<MusicControllerExcute>();
            foreach (var controllerInfo in controllerInfos)
            {
                musicControllerExcutes.AddRange(controllerInfo.ItemXmlInfo.Excutes.OfType<MusicControllerExcute>());
            }
            return musicControllerExcutes;
        }

        private static void GetAllControllerInfo(TreeItemBase itemBase, ref List<ControllerInfo> controllerInfos)
        {
            var controllerInfo = itemBase as ControllerInfo;
            if (controllerInfo != null)
            {
                controllerInfos.Add(controllerInfo);
            }
            foreach (var item in itemBase.Items)
            {
                GetAllControllerInfo(item, ref controllerInfos);
            }
        }

        public static string GetBoardPortName(int deviceNumber, BoardType boardType)
        {
            var boardInfos = GetBoardInfos(boardType);
            foreach (var boardInfo in boardInfos)
            {
                if (boardInfo.DeviceNumber == deviceNumber)
                {
                    return boardInfo.SignName;
                }
            }
            return string.Empty;
        }

        public static string GetProjectorPortName(string mark)
        {
            var markInfo = Project.ItemXmlInfo.ProjectorGroup.ProjectorMarkMatchInfos.FirstOrDefault(s => s.Mark == mark);
            if (markInfo != null)
            {
                return markInfo.SignName;
            }
            return string.Empty;
        }

        public static string GetMusicServiceAddress(string musicMark)
        {
            var musicServiceMark = Project.ItemXmlInfo.MusicGroup.MusicMarkMatchInfos.FirstOrDefault(s => s.Mark == musicMark);
            if (musicServiceMark != null)
            {
                var serviceAddressMark = musicServiceMark.ServiceAddressMark;
                var serviceAddressInfo = Project.ItemXmlInfo.MusicGroup.MusicServiceAddressInfos.FirstOrDefault(s => s.Mark == serviceAddressMark);
                if (serviceAddressInfo != null)
                {
                    return serviceAddressInfo.ServiceAddress;
                }
            }
            return string.Empty;
        }

        public static IEnumerable<string> GetMusicAllServiceAddress()
        {
            return Project.ItemXmlInfo.MusicGroup.MusicServiceAddressInfos.GroupBy(s => s.ServiceAddress).Select(s => s.Key).ToList();
        }

        public static string GetNotificationServiceAddress(string appMark)
        {
            var appServiceMark = Project.ItemXmlInfo.NotificationGroup.MarkMatchInfos.FirstOrDefault(s => s.Mark == appMark);
            if (appServiceMark != null)
            {
                var serviceAddressMark = appServiceMark.ServiceAddressMark;
                var serviceAddressInfo = Project.ItemXmlInfo.NotificationGroup.AppNitificationServiceAddressInfos.FirstOrDefault(s => s.Mark == serviceAddressMark);
                if (serviceAddressInfo != null)
                {
                    return serviceAddressInfo.ServiceAddress;
                }
            }
            return string.Empty;
        }

        public static string GetProjectorServiceAddress(string appMark)
        {
            var appServiceMark = Project.ItemXmlInfo.ProjectorGroup.ProjectorMarkMatchInfos.FirstOrDefault(s => s.Mark == appMark);
            if (appServiceMark != null)
            {
                var serviceAddressMark = appServiceMark.ServiceAddressMark;
                var serviceAddressInfo = Project.ItemXmlInfo.ProjectorGroup.ProjectorServiceAddressInfos.FirstOrDefault(s => s.Mark == serviceAddressMark);
                if (serviceAddressInfo != null)
                {
                    return serviceAddressInfo.ServiceAddress;
                }
            }
            return string.Empty;
        }

        private static List<BoardXmlInfo> GetBoardInfos(BoardType boardType)
        {
            List<BoardXmlInfo> boardInfos;
            switch (boardType)
            {
                case BoardType.InputA:
                    boardInfos = Project.ItemXmlInfo.InputBoardGroup.BoardInfos.ToList();
                    break;
                case BoardType.OutputA:
                    boardInfos = Project.ItemXmlInfo.OutputBoardGroup.BoardInfos.ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("boardType", boardType, null);
            }
            return boardInfos;
        }

        public static ControllerXmlInfo GetControllerXmlInfo(ProjectInfo project, string cid)
        {
            var controllXmlInfo = FindControllerXmlInfo(project, cid);
            return controllXmlInfo;
        }

        private static ControllerXmlInfo FindControllerXmlInfo(TreeItemBase parent, string cid)
        {
            var controllerInfo = parent as ControllerInfo;
            if ((controllerInfo != null) && (controllerInfo.ItemXmlInfo.CID == cid))
            {
                return controllerInfo.ItemXmlInfo;
            }
            foreach (var item in parent.Items)
            {
                var citem = FindControllerXmlInfo(item, cid);
                if (citem != null)
                {
                    return citem;
                }
            }
            return null;
        }

        public static bool FindServiceAddressMark(ProjectInfo project, BoardType boardType, int deviceNumber, out string serviceAddressMark)
        {
            ObservableCollection<BoardXmlInfo> boardXmlInfos;
            switch (boardType)
            {
                case BoardType.InputA:
                    boardXmlInfos = project.ItemXmlInfo.InputBoardGroup.BoardInfos;
                    break;
                case BoardType.OutputA:
                    boardXmlInfos = project.ItemXmlInfo.OutputBoardGroup.BoardInfos;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            serviceAddressMark = string.Empty;
            foreach (var boardXmlInfo in boardXmlInfos)
            {
                if (boardXmlInfo.DeviceNumber == deviceNumber)
                {
                    serviceAddressMark = boardXmlInfo.ServiceAddressMark;
                    break;
                }
            }
            return !serviceAddressMark.IsNullOrEmpty();
        }

        public static bool FindPortName(ProjectInfo project, BoardType boardType, int deviceNumber, out string portName)
        {
            ObservableCollection<BoardXmlInfo> boardXmlInfos;
            switch (boardType)
            {
                case BoardType.InputA:
                    boardXmlInfos = project.ItemXmlInfo.InputBoardGroup.BoardInfos;
                    break;
                case BoardType.OutputA:
                    boardXmlInfos = project.ItemXmlInfo.OutputBoardGroup.BoardInfos;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            portName = string.Empty;
            foreach (var boardXmlInfo in boardXmlInfos)
            {
                if (boardXmlInfo.DeviceNumber == deviceNumber)
                {
                    portName = boardXmlInfo.SignName;
                    break;
                }
            }
            return !portName.IsNullOrEmpty();
        }

        public static void GetAllController(ref List<ControllerInfo> controllerInfos, TreeItemBase treeItemBase)
        {
            var controllerInfo = treeItemBase as ControllerInfo;
            if (controllerInfo != null)
            {
                controllerInfos.Add(controllerInfo);
            }
            foreach (var treeItem in treeItemBase.Items)
            {
                GetAllController(ref controllerInfos, treeItem);
            }
        }

        public static List<BoardControllerExcute> GetAllBoardExcutes(List<ControllerInfo> controllerInfos)
        {
            var list = new List<BoardControllerExcute>();
            foreach (var controllerInfo in controllerInfos)
            {
                list.AddRange(controllerInfo.ItemXmlInfo.Excutes.OfType<BoardControllerExcute>());
            }
            return list;
        }

        public static List<string> GetBoardExcuteDescriptioins(int deviceNumber, int number, ProjectInfo projectInfo)
        {
            var descriptions = new List<string>();
            var controllerInfos = new List<ControllerInfo>();
            GetAllController(ref controllerInfos, projectInfo);
            var boardControllerExcutes = GetAllBoardExcutes(controllerInfos);
            foreach (var excute in boardControllerExcutes)
            {
                var boardItemInfo = excute.BoardItemInfos.FirstOrDefault(s => (s.DeviceNumber == deviceNumber) && (s.Number == number));
                if (boardItemInfo != null)
                {
                    descriptions.Add(excute.Description);
                }
            }
            return descriptions;
        }
    }
}