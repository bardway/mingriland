// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#endregion

namespace TLAuto.Base.Extensions
{
    public static class EnumerableExtensions
    {
        public static void Up<T>(this ObservableCollection<T> list, T source)
        {
            var oldIndex = list.IndexOf(source);
            if (oldIndex != 0)
            {
                list.Move(oldIndex, oldIndex - 1);
            }
        }

        public static void Down<T>(this ObservableCollection<T> list, T source)
        {
            var oldIndex = list.IndexOf(source);
            if (oldIndex != list.Count - 1)
            {
                list.Move(oldIndex, oldIndex + 1);
            }
        }

        public static void Add<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = new List<TValue>();
            }

            dictionary[key].Add(value);
        }

        public static void Add<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<TValue>());
            }
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
            }
            dictionary.Add(key, value);
        }

        public static List<TValue> ToValueList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary)
        {
            var ret = new List<TValue>();
            foreach (var value in dictionary.Values)
            {
                ret.AddRange(value);
            }
            return ret;
        }

        public static TValue GetValueIfNotExistRetNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue outValue;
            if (!dictionary.TryGetValue(key, out outValue))
            {
                return default(TValue);
            }
            return outValue;
        }

        public static int RemoveAll<T>(this ObservableCollection<T> collctions, Predicate<T> pre)
        {
            //Please check it because it is by Guifan~haha
            //find all the items
            var removedItems = new ObservableCollection<T>();
            foreach (var c in collctions.Where(c => pre(c)))
            {
                removedItems.Add(c);
            }
            //delete
            foreach (var item in removedItems)
            {
                collctions.Remove(item);
            }
            return removedItems.Count;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="newList"></param>
        /// <param name="equalsAction"></param>
        /// <param name="modifyAction"></param>
        /// <param name="order"></param>
        public static void UpdateSourceWithoutRemove<T>(this IList<T> src, IEnumerable<T> newList, Func<T, T, bool> equalsAction, Action<T, T> modifyAction, Action<T> beforeAddNewAction = null, bool order = false) where T: class
        {
            foreach (var newOne in newList)
            {
                var find = src.FirstOrDefault(i => equalsAction(i, newOne));
                if (find != null)
                {
                    modifyAction(find, newOne);
                    continue;
                }
                //表示新添加的任务,插在第一个位置
                if (beforeAddNewAction != null)
                {
                    beforeAddNewAction(newOne);
                }
                if (order)
                {
                    src.Add(newOne);
                }
                else
                {
                    src.Insert(0, newOne);
                }
            }
        }

        public static void GetDifferent<T>(this IEnumerable<T> src, IEnumerable<T> newList, Func<T, T, bool> equalsAction, out List<T> removeItems, out List<T> addItems)
        {
            var tSrc = src as List<T> ?? src.ToList();
            var tNewList = newList as List<T> ?? newList.ToList();

            //get remove list
            removeItems = new List<T>();
            foreach (var srcItem in tSrc)
            {
                if (!tNewList.Exists(i => equalsAction(i, srcItem)))
                {
                    removeItems.Add(srcItem);
                }
            }

            //get add list
            addItems = new List<T>();
            foreach (var newItem in newList)
            {
                if (!tSrc.Exists(i => equalsAction(i, newItem)))
                {
                    addItems.Add(newItem);
                }
            }
        }

        public static void UpdateSource<T>
        (
            this IList<T> src,
            IEnumerable<T> newList,
            Func<T, T, bool> equalsAction,
            Action<T, T> modifyAction = null,
            Action<T> beforeAddNewAction = null,
            bool order = false) where T: class
        {
            IList<T> removeList;
            UpdateSource(src, newList, equalsAction, out removeList, modifyAction, beforeAddNewAction, order);
        }

        public static void UpdateSource<T>
        (
            this IList<T> src,
            IEnumerable<T> newList,
            Func<T, T, bool> equalsAction,
            out IList<T> removeList,
            Action<T, T> modifyAction = null,
            Action<T> beforeAddNewAction = null,
            bool order = false) where T: class
        {
            var tNewList = newList as IList<T> ?? newList.ToList();

            foreach (var newOne in tNewList)
            {
                var find = src.FirstOrDefault(i => equalsAction(i, newOne));
                if (find != null)
                {
                    if (modifyAction != null)
                    {
                        modifyAction(find, newOne);
                    }
                    continue;
                }
                //表示新添加的任务,插在第一个位置
                if (beforeAddNewAction != null)
                {
                    beforeAddNewAction(newOne);
                }
                if (order)
                {
                    src.Add(newOne);
                }
                else
                {
                    src.Insert(0, newOne);
                }
            }

            removeList = new List<T>();
            foreach (var old in src)
            {
                var found = tNewList.Any(i => equalsAction(i, old));
                if (!found)
                {
                    removeList.Add(old);
                }
            }

            foreach (var item in removeList)
            {
                src.Remove(item);
            }
        }

        /// <summary>
        /// parse the ip from SRVI to xxx.xxx.xxx.xxx format.
        /// </summary>
        /// <param name="ipnum"></param>
        /// <returns></returns>
        public static string ExtUintToStringAsIp(this uint ipnum)
        {
            var ipstr = Convert.ToString(ipnum, 16);
            if (ipstr.Length < 8)
            {
                var zero = "";
                for (var i = 0; i < 8 - ipstr.Length; i++)
                {
                    zero += "0";
                }
                ipstr = zero + ipstr;
            }

            char[] a = {ipstr[0], ipstr[1]};
            char[] b = {ipstr[2], ipstr[3]};
            char[] c = {ipstr[4], ipstr[5]};
            char[] d = {ipstr[6], ipstr[7]};

            var astr = Convert.ToInt32(string.Concat(a), 16);
            var bstr = Convert.ToInt32(string.Concat(b), 16);
            var cstr = Convert.ToInt32(string.Concat(c), 16);
            var dstr = Convert.ToInt32(string.Concat(d), 16);

            return string.Format("{0}.{1}.{2}.{3}", astr, bstr, cstr, dstr);
        }

        public static byte[] ExtIpToByte(this string ip)
        {
            var ipStrs = ip.Split('.');
            var ret = new byte[4];
            ret[0] = byte.Parse(ipStrs[0]);
            ret[1] = byte.Parse(ipStrs[1]);
            ret[2] = byte.Parse(ipStrs[2]);
            ret[3] = byte.Parse(ipStrs[3]);
            return ret;
        }

        public static string ExtIpToString(this byte[] ip)
        {
            return string.Format("{0}.{1}.{2}.{3}", ip[0], ip[1], ip[2], ip[3]);
        }

        /// <summary>
        /// select the keyword
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string ExtSplitKeywordsOfClip(this string keywords, ushort index)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                return string.Empty;
            }
            var keywordarray = keywords.Split(' ');

            return keywordarray.Length - 1 < index
                       ? string.Empty
                       : keywordarray[index].Length > 12
                           ? keywordarray[index].Substring(0, 12)
                           : keywordarray[index];
        }

        public static int ToInt32(this string inpoint, int index, int length)
        {
            int temp;
            var intstr = inpoint.Substring(index, length);
            int.TryParse(intstr, out temp);
            return temp;
        }

        public static bool SequenceEqualAfterSort<T>(this List<T> src, List<T> list)
        {
            src.Sort();
            list.Sort();
            return src.SequenceEqual(list);
        }

        /// <summary>
        /// Page, pageIndex start with 1.
        /// </summary>
        public static List<T> Page<T>(this IList<T> list, int pageIndex, int pageSize)
        {
            var newlists = new List<T>();
            if (pageIndex < 1)
            {
                return newlists;
            }
            var startIndex = (pageIndex - 1) * pageSize;
            var endIndex = startIndex + pageSize;
            for (var i = startIndex; i < endIndex; i++)
            {
                if (list.Count > i)
                {
                    newlists.Add(list[i]);
                }
            }
            return newlists;
        }

        public static V GetValueOrDefaut<K, V>(this ConcurrentDictionary<K, V> dic, K key)
        {
            V value;
            if (!dic.TryGetValue(key, out value))
            {
                return default(V);
            }
            return value;
        }

        /// <summary>
        /// 将连续的int分组, 返回从大到小排列组。
        /// </summary>
        /// <returns>返回的组，从大到小排列</returns>
        public static List<List<int>> ToContinuousGroup(this List<int> numbers)
        {
            var list = new List<List<int>>(); //存放分组
            var previousValue = -1; //记录前一个值，以便和当前值比较
            var group = new List<int>(); //保存一个分组
            numbers = numbers.OrderBy(i => i).ToList();
            for (var i = 0; i < numbers.Count; i++)
            {
                if (previousValue != numbers[i] - 1) //与前一个数字不连续，需要重新分组
                {
                    if (group.Count != 0)
                    {
                        list.Add(group); //保存之前的组
                    }
                    group = new List<int>();
                }

                //记录当前值并添加到当前分组中
                previousValue = numbers[i];
                group.Add(numbers[i]);

                //保存最后一组
                if (i == numbers.Count - 1)
                {
                    list.Add(group);
                }
            }
            return list;
        }

        public static void Sort<T>(this ObservableCollection<T> collection) where T: IComparable
        {
            var sorted = collection.OrderBy(x => x).ToList();
            for (var i = 0; i < sorted.Count(); i++)
            {
                collection.Move(collection.IndexOf(sorted[i]), i);
            }
        }

        public static void Sort<T>(this ObservableCollection<T> collection, IComparer<T> comparer)
        {
            var sorted = collection.ToList();
            sorted.Sort(comparer);
            for (var i = 0; i < sorted.Count(); i++)
            {
                collection.Move(collection.IndexOf(sorted[i]), i);
            }
        }

        public static List<T> CopyTo<T>(this IList<T> list, int startIndex, int length)
        {
            if (startIndex + length > list.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            var ret = new List<T>();
            for (var i = startIndex; i < length; i++)
            {
                ret.Add(list[i]);
            }

            return ret;
        }

        public static string ToText<T>(this IList<T> list)
        {
            return list.Aggregate(string.Empty, (current, t) => current + " " + t.ToString());
        }

        private class ObservableCollectionComparer<T> : IComparer<T>
        {
            private readonly Comparison<T> comparison;

            public ObservableCollectionComparer(Comparison<T> comparison)
            {
                this.comparison = comparison;
            }

            #region IComparer<T> Members
            public int Compare(T x, T y)
            {
                return comparison.Invoke(x, y);
            }
            #endregion
        }
    }
}