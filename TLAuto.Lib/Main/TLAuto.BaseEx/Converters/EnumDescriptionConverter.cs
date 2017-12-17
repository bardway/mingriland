// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.BaseEx.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum)
            {
                var myEnum = (Enum)value;
                var description = myEnum.GetEnumAttribute<DescriptionAttribute>().Description;
                return description;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}