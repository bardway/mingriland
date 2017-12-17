// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
#endregion

namespace TLAuto.BaseEx.Extensions
{
    public class AutoRefreshCollectionViewSource : CollectionViewSource
    {
        protected override void OnSourceChanged(object oldSource, object newSource)
        {
            if (oldSource != null)
            {
                SubscribeSourceEvents(oldSource, true);
            }
            if (newSource != null)
            {
                SubscribeSourceEvents(newSource, false);
            }
            base.OnSourceChanged(oldSource, newSource);
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var refresh = false;
            foreach (var sort in SortDescriptions)
            {
                if (sort.PropertyName == e.PropertyName)
                {
                    refresh = true;
                    break;
                }
            }
            if (!refresh)
            {
                foreach (var group in GroupDescriptions)
                {
                    var propertyGroup = group as PropertyGroupDescription;
                    if ((propertyGroup != null) && (propertyGroup.PropertyName == e.PropertyName))
                    {
                        refresh = true;
                        break;
                    }
                }
            }
            if (refresh)
            {
                View.Refresh();
            }
        }

        private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                SubscribeItemsEvents(e.NewItems, false);
            }
            else
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    SubscribeItemsEvents(e.OldItems, true);
                }
                else
                {
                    if (e.Action == NotifyCollectionChangedAction.Replace)
                    {
                        SubscribeItemsEvents(e.OldItems, true);
                        SubscribeItemsEvents(e.NewItems, false);
                    }
                }
            }
            //else
            //{
            //    Debug.Assert(false);
            //}
        }

        private void SubscribeItemEvents(object item, bool remove)
        {
            var notify = item as INotifyPropertyChanged;
            if (notify != null)
            {
                if (remove)
                {
                    notify.PropertyChanged -= Item_PropertyChanged;
                }
                else
                {
                    notify.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void SubscribeItemsEvents(IEnumerable items, bool remove)
        {
            foreach (var item in items)
            {
                SubscribeItemEvents(item, remove);
            }
        }

        private void SubscribeSourceEvents(object source, bool remove)
        {
            var notify = source as INotifyCollectionChanged;
            if (notify != null)
            {
                if (remove)
                {
                    notify.CollectionChanged -= Source_CollectionChanged;
                }
                else
                {
                    notify.CollectionChanged += Source_CollectionChanged;
                }
            }
            SubscribeItemsEvents((IEnumerable)source, remove);
        }
    }
}