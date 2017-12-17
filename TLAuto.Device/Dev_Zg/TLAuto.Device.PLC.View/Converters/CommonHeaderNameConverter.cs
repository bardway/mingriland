// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Globalization;
using System.Windows.Data;
#endregion

namespace TLAuto.Device.PLC.View.Converters
{
    public class CommonHeaderNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var constName = values[0].ToString();
            var variableName = values[1].ToString();
            return $"{parameter}{constName}（{variableName}）";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}