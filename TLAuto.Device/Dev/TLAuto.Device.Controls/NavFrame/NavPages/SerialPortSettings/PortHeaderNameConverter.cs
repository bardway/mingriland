// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Globalization;
using System.Windows.Data;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.Controls.NavFrame.NavPages.SerialPortSettings
{
    public class PortHeaderNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var portName = values[0].ToString();
            var isUsed = values[1].ToBoolean();
            return isUsed ? $"{portName}（使用中）" : portName;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}