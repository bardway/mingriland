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
    public class VolumeValueToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var volume = (double)value;
            if ((volume > 0) && (volume <= 0.5))
            {
                return 1;
            }
            if ((volume > 0.5) && (volume <= 0.75))
            {
                return 2;
            }
            if ((volume > 0.75) && (volume <= 1))
            {
                return 3;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}