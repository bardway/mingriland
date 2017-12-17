// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.ControlEx.App.Models.TreeModels;
#endregion

namespace TLAuto.ControlEx.App.Models
{
    public class ProjectInfo : TreeItemBase
    {
        public ProjectInfo(string fileName, string location)
            : base(null, fileName, location)
        {
            LoadItemXml<ProjectXmlInfo>(FullPath);
        }

        public ProjectXmlInfo ItemXmlInfo => (ProjectXmlInfo)ItemXml;

        public override bool HasTabSettingsItem => true;
    }
}