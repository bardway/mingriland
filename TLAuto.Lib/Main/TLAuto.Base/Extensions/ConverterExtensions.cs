// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Text;
#endregion

namespace TLAuto.Base.Extensions
{
    public static class ConverterExtensions
    {
        public static string ToStringWithNull<T>(this T obj)
        {
            try
            {
                return Convert.ToString(obj);
            }
            catch
            {
                return default(string);
            }
        }

        /// <summary>
        /// An object converted into byte, if obj is null return to byte default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Source</param>
        /// <returns>Return byte,if converted failure return to byte default value.</returns>
        public static byte ToByte<T>(this T obj)
        {
            try
            {
                return Convert.ToByte(obj);
            }
            catch
            {
                return default(byte);
            }
        }

        /// <summary>
        /// An object converted into int32, if obj is null return to int32 default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Source</param>
        /// <returns>Return int32, if converted failure return to int32 default value.</returns>
        public static int ToInt32<T>(this T obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return default(int);
            }
        }

        /// <summary>
        /// An object converted into int64(long), if obj is null return to int64 default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Source</param>
        /// <returns>Return int64, if converted failure return to int64 default value.</returns>
        public static long ToInt64<T>(this T obj)
        {
            try
            {
                return Convert.ToInt64(obj);
            }
            catch
            {
                return default(int);
            }
        }

        /// <summary>
        /// An object converted into uint32, if obj is null return to uint32 default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Source</param>
        /// <returns>Return uint32, if converted failure return to uint32 default value.</returns>
        public static uint ToUInt32<T>(this T obj)
        {
            try
            {
                return Convert.ToUInt32(obj);
            }
            catch
            {
                return default(uint);
            }
        }

        /// <summary>
        /// An object converted into uint64, if obj is null return to uint64 default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Source</param>
        /// <returns>Return uint64, if converted failure return to uint64 default value.</returns>
        public static ulong ToUInt64<T>(this T obj)
        {
            try
            {
                return Convert.ToUInt64(obj);
            }
            catch
            {
                return default(ulong);
            }
        }

        /// <summary>
        /// An object converted into bool, if obj is null return to bool default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Source</param>
        /// <returns>Return bool, if converted failure return to bool default value.</returns>
        public static bool ToBoolean<T>(this T obj)
        {
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return default(bool);
            }
        }

        /// <summary>
        /// An object converted into decimal, if obj is null return to decimal default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Source</param>
        /// <returns>Return decimal, if converted failure return to decimal default value.</returns>
        public static decimal ToDecimal<T>(this T obj)
        {
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return default(decimal);
            }
        }

        /// <summary>
        /// An object converted into double, if obj is null return to double default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Source</param>
        /// <returns>Return double, if converted failure return to double default value.</returns>
        public static double ToDouble<T>(this T obj)
        {
            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return default(double);
            }
        }

        /// <summary>
        /// An object converted into single, if obj is null return to single default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Source</param>
        /// <returns>Return single, if converted failure return to single default value.</returns>
        public static float ToSingle<T>(this T obj)
        {
            try
            {
                return Convert.ToSingle(obj);
            }
            catch
            {
                return default(float);
            }
        }

        /// <summary>
        /// 把byte[]转换成易于阅读的字符串。
        /// </summary>
        /// <param name="bytes">返回转换的字符串。</param>
        public static string ToDispString(this byte[] bytes)
        {
            if (bytes == null)
            {
                return "null";
            }

            var row = 0;
            var sb = new StringBuilder();
            sb.Append("          0  1  2  3  4  5  6  7    8  9  a  b  c  d  e  f\r\n");
            sb.Append("          -------------------------------------------------\r\n");
            sb.Append(row.ToString("x8") + ": ");

            var index = 0;
            foreach (var b in bytes)
            {
                sb.Append(b.ToHex() + " ");
                index++;
                if ((index % 8 == 0) && (index % 16 != 0))
                {
                    sb.Append("  ");
                }
                if (index % 16 == 0)
                {
                    row += 16;
                    sb.Append(string.Format("\r\n{0}: ", row.ToString("x8")));
                }
            }

            sb.Append("\r\n          -------------------------------------------------");
            return sb.ToString();
        }

        public static string ToHex(this int str)
        {
            return Convert.ToString(str, 16).PadLeft(2, '0');
        }

        public static string ToPadLeft(this int number, int totalWidth = 2)
        {
            return number.ToString().PadLeft(totalWidth, '0');
        }

        public static string ToPadRight(this int number, int totalWidth = 2)
        {
            return number.ToString().PadRight(totalWidth, '0');
        }

        public static string ToHexWithoutPadLeft(this int str)
        {
            return Convert.ToString(str, 16);
        }
    }
}