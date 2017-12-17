// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Globalization;
using System.Windows.Data;

using TLAuto.ControlEx.App.Common;
#endregion

namespace TLAuto.ControlEx.App.Converter
{
    public class CIDToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cid = (string)value;
            var controllerXmlInfo = ProjectHelper.GetControllerXmlInfo(ProjectHelper.Project, cid);
            return controllerXmlInfo != null ? controllerXmlInfo.Description : cid;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}