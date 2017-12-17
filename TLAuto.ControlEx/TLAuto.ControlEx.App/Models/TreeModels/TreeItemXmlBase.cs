// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
#endregion

namespace TLAuto.ControlEx.App.Models.TreeModels
{
    [XmlInclude(typeof(ProjectXmlInfo))]
    [XmlInclude(typeof(ControllerXmlInfo))]
    [XmlRoot("ItemXml")]
    public abstract class TreeItemXmlBase : ObservableObject { }
}