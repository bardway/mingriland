// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Globalization;
using System.Windows.Data;
#endregion

namespace TLAuto.Music.Controls.Converter
{
    public class TimeSpanToDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var totalSencond = (double)value;
            var ts = TimeSpan.FromSeconds(totalSencond);
            var hours = ts.Hours.ToString().PadLeft(2, '0');
            var minutes = ts.Minutes.ToString().PadLeft(2, '0');
            var seconds = ts.Seconds.ToString().PadLeft(2, '0');
            return string.Format("{0}:{1}:{2}", hours, minutes, seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}