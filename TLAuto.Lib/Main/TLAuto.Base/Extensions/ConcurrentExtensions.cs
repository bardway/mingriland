// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Concurrent;
#endregion

namespace TLAuto.Base.Extensions
{
    public static class ConcurrentExtensions
    {
        /// <summary>
        /// 清空队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="concurrentQueue"></param>
        public static void Clear<T>(this ConcurrentQueue<T> concurrentQueue)
        {
            while (!concurrentQueue.IsEmpty)
            {
                T remove;
                concurrentQueue.TryDequeue(out remove);
            }
        }

        /// <summary>
        /// Clear all items.
        /// </summary>
        public static void Clear<T>(this ConcurrentBag<T> concurrentBag)
        {
            while (!concurrentBag.IsEmpty)
            {
                T someItem;
                concurrentBag.TryTake(out someItem);
            }
        }
    }
}