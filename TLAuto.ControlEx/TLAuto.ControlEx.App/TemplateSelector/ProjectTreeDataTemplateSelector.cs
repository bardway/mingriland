// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;
using System.Windows.Controls;

using TLAuto.ControlEx.App.Models;
#endregion

namespace TLAuto.ControlEx.App.TemplateSelector
{
    public class ProjectTreeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProjectTreeItemDataTemplate { set; get; }

        public DataTemplate FolderTreeItemDataTemplate { set; get; }

        public DataTemplate ControllerTreeItemDataTemplate { set; get; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ProjectInfo)
            {
                return ProjectTreeItemDataTemplate;
            }
            if (item is FolderInfo)
            {
                return FolderTreeItemDataTemplate;
            }
            if (item is ControllerInfo)
            {
                return ControllerTreeItemDataTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}