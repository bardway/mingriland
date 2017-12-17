// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

using TLAuto.BaseEx.Extensions;
#endregion

namespace TLAuto.BaseEx.Converters
{
    [ValueConversion(typeof(IList), typeof(IEnumerable))]
    public class CollectionViewFactoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as IList;
            if (collection != null)
            {
                var viewSource = new AutoRefreshCollectionViewSource();
                viewSource.Source = collection;
                //var view = new ListCollectionView(collection);
                var sortNames = parameter.ToString().Split('|');
                foreach (var sortName in sortNames)
                {
                    var sort = new SortDescription(sortName, ListSortDirection.Ascending);
                    viewSource.SortDescriptions.Add(sort);
                }
                return viewSource.View;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}