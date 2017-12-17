// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Text.RegularExpressions;
#endregion

namespace TLAuto.Base.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 2个时间段是否有交集
        /// </summary>
        /// <param name="t1Start"></param>
        /// <param name="t1End"></param>
        /// <param name="t2Start"></param>
        /// <param name="t2End"></param>
        /// <returns>True，表示有交集</returns>
        public static bool CompareIntersection(DateTime t1Start, DateTime t1End, DateTime t2Start, DateTime t2End)
        {
            var ts1 = t2Start - t1Start;
            TimeSpan ts2;
            if (ts1.Ticks > 0)
            {
                ts2 = t2Start - t1End;
                if (ts2.Ticks >= 0)
                {
                    return false;
                }
                return true;
            }
            ts2 = t1Start - t2End;
            if (ts2.Ticks >= 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 是否为日期型字符串
        /// </summary>
        /// <param name="source">日期字符串(2008-05-08)</param>
        /// <returns></returns>
        public static bool IsDate(this string source)
        {
            return Regex.IsMatch(source, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
        }

        /// <summary>
        /// 是否为时间型字符串
        /// </summary>
        /// <param name="source">时间字符串(15:00:00)</param>
        /// <returns></returns>
        public static bool IsTime(this string source)
        {
            return Regex.IsMatch(source, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }

        /// <summary>
        /// 是否为日期+时间型字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDateAndTime(this string source)
        {
            return Regex.IsMatch(source, @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$ ");
        }

        public static DateTime ToUtcDateTime(this string s)
        {
            return DateTime.Now;
        }

        public static string ToStringWithStandard(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToStringWithT(this DateTime dt)
        {
            return dt.ToString("'yyyy'-'MM'-'dd'T'HH'L'mm':'ss'");
        }

        public static string ToUtcString(this DateTime dt)
        {
            return null;
        }
    }
}