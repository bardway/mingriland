// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
#endregion

namespace TLAuto.BaseEx.Converters
{
    public class CollectionViewSortConverter : IValueConverter
    {
        static CollectionViewSortConverter()
        {
            Instance = new CollectionViewSortConverter();
        }

        public static CollectionViewSortConverter Instance { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sortPropertys = parameter.ToString().Split('|');
            var view = new ListCollectionView((IList)value);
            foreach (var sortProperty in sortPropertys)
            {
                view.SortDescriptions.Add(new SortDescription(sortProperty, ListSortDirection.Ascending));
            }
            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var view = (CollectionView)value;
            return view.SourceCollection;
        }
    }
}