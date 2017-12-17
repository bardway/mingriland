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
    public class ProjectTabDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProjectTabItemDataTemplate { set; get; }

        public DataTemplate ControllerTabItemDataTemplate { set; get; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ProjectInfo)
            {
                return ProjectTabItemDataTemplate;
            }
            if (item is ControllerInfo)
            {
                return ControllerTabItemDataTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}