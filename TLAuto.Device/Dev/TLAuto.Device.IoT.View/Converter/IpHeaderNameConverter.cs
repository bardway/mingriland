// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

using TLAuto.Base.Extensions;
using TLAuto.Device.IoT.View.Models.Enums;
#endregion

namespace TLAuto.Device.IoT.View.Converter
{
    public class IpHeaderNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var signName = values[0];
            var ip = values[1];
            var port = values[2];
            var iotSocketType = ((IoTSocketType)values[3]).GetEnumAttribute<DescriptionAttribute>().Description;
            return $"{signName}({parameter}{ip}:{port})-{iotSocketType}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}