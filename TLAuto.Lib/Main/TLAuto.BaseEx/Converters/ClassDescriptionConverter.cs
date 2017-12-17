// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
#endregion

namespace TLAuto.BaseEx.Converters
{
    public class ClassDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var customAttributes = value.GetType().GetCustomAttributes(false);
            var list = customAttributes.Where(s => s is DescriptionAttribute).ToList();
            if (list.Count == 1)
            {
                return ((DescriptionAttribute)list[0]).Description;
            }
            return "没有找到描述信息。";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}