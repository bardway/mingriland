// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
#endregion

namespace TLAuto.Base.Extensions
{
    public static class ByteExtensions
    {
        #region Hash
        /// <summary>
        /// 使用指定算法Hash
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hashName"></param>
        /// <returns></returns>
        public static byte[] Hash(this byte[] data, string hashName = null)
        {
            var algorithm = hashName.IsNullOrEmpty()
                                ? HashAlgorithm.Create()
                                : HashAlgorithm.Create(hashName);
            return algorithm.ComputeHash(data);
        }
        #endregion

        #region 复制
        /// <summary>
        /// 复制byte数组到指定byte数组。
        /// </summary>
        /// <param name="src">源数组。</param>
        /// <param name="target">目标数组。</param>
        /// <param name="srcIndex">开始的索引号。</param>
        /// <returns>返回目标数组。</returns>
        public static void CopyIndex(this byte[] src, byte[] target, long srcIndex)
        {
            var p = 0;
            for (var i = srcIndex; i < src.Length; i++)
            {
                target[p] = src[i];
                p++;
            }
        }
        #endregion

        #region 转换为十六进制字符串
        /// <summary>
        /// 转换为十六进制字符串
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToHex(this byte b)
        {
            return b.ToString("X2");
        }

        public static string ToBin(this byte b)
        {
            return Convert.ToString(b, 2).PadLeft(8, '0');
        }

        /// <summary>
        /// 转换为十六进制字符串（每个字符是连起的，如果要方便阅读请调用带字符参数的此方法）
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHex(this IEnumerable<byte> bytes)
        {
            return ToHex(bytes, string.Empty);
        }

        /// <summary>
        /// 转换为十六进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="split">分隔符</param>
        /// <returns></returns>
        public static string ToHex(this IEnumerable<byte> bytes, string split)
        {
            var isNull = split.IsNullOrEmpty();
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
                if (!isNull)
                {
                    sb.Append(split);
                }
            }
            return isNull
                       ? sb.ToString()
                       : sb.ToString().DelLastChar(split);
        }

        /// <summary>
        /// 16进制字符串转字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="split">分隔符</param>
        /// <returns></returns>
        public static byte[] HexStrToBytes(this string hexString, string split)
        {
            hexString = hexString.Replace(split, "");

            if (hexString.Length % 2 != 0)
            {
                hexString += " ";
            }

            var returnBytes = new byte[hexString.Length / 2];

            for (var i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }
        #endregion

        #region 位运算
        /// <summary>
        /// index从0开始
        /// 获取取第index是否为1
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool GetBit(this byte b, int index)
        {
            return (b & (1 << index)) > 0;
        }

        /// <summary>
        /// 将第index位设为1
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static byte SetBit(this byte b, int index)
        {
            b |= (byte)(1 << index);
            return b;
        }

        /// <summary>
        /// 将第index位设为0
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static byte ClearBit(this byte b, int index)
        {
            b &= (byte)((1 << 8) - 1 - (1 << index));
            return b;
        }

        /// <summary>
        /// 将第index位取反
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static byte ReverseBit(this byte b, int index)
        {
            b ^= (byte)(1 << index);
            return b;
        }
        #endregion

        #region 数据类型转换
        /// <summary>
        /// 将指定字符串转成指定长度的bytes。
        /// </summary>
        public static byte[] ToBytes(this string str, int size, Encoding encoding)
        {
            var bytes = encoding.GetBytes(str);
            var ret = new byte[size];
            bytes.CopyTo(ret, 0);
            return ret;
        }

        /// <summary>
        /// 将指定字符串转成指定长度的bytes。
        /// </summary>
        public static byte[] ToBytes(this string str, int size)
        {
            return str.ToBytes(size, Encoding.Default);
        }

        /// <summary>
        /// 将指定字符串转成指定长度的bytes。
        /// </summary>
        public static byte[] ToBytesUTF8(this string str, int size)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var ret = new byte[size];
            bytes.CopyTo(ret, 0);
            return ret;
        }

        /// <summary>
        /// 将指定的字节数组转换为Base64字符串
        /// </summary>
        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static string ToDefaultCodingString(this byte[] bytes)
        {
            var s = Encoding.Default.GetString(bytes);
            return s.TrimEndZero();
        }

        public static string ToDefaultCodingString(this ushort[] bytes)
        {
            var bs = new byte[bytes.Length];
            bytes.CopyTo(bs, 0);
            var s = Encoding.Default.GetString(bs);
            return s.TrimEndZero();
        }

        /// <summary>
        /// 转换为指定编码的字符串
        /// </summary>
        /// <param name="data">字节数组</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static string Decode(this byte[] data, Encoding encoding)
        {
            return encoding.GetString(data);
        }

        /// <summary>
        /// 转换为内存流
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(this byte[] data)
        {
            return new MemoryStream(data);
        }

        /// <summary>
        /// 根据指定的长度进行自动补位
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static short ConvertInt16(this byte[] data)
        {
            var datas = ConvertBytesToLength(data, 2);
            return BitConverter.ToInt16(datas, 0);
        }

        /// <summary>
        /// 根据指定的长度进行自动补位
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int ConvertInt32(this byte[] data)
        {
            var datas = ConvertBytesToLength(data, 4);
            return BitConverter.ToInt32(datas, 0);
        }

        /// <summary>
        /// 根据指定的长度进行自动补位
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long ConvertInt64(this byte[] data)
        {
            var datas = ConvertBytesToLength(data, 8);
            return BitConverter.ToInt64(datas, 0);
        }

        /// <summary>
        /// 根据长度自动补位
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataSize"></param>
        /// <returns></returns>
        private static byte[] ConvertBytesToLength(byte[] data, int dataSize)
        {
            if (data.Length == dataSize)
            {
                return data;
            }
            var bytes = new byte[dataSize];
            for (var i = 0; i < dataSize; i++)
            {
                if (data.Length == i + 1)
                {
                    bytes[i] = data[i];
                }
                else
                {
                    bytes[i] = 0;
                }
            }
            return bytes;
        }
        #endregion

        #region 序列化和反序列化
        /// <summary>
        /// 将对象转为字节数组
        /// </summary>
        /// <typeparam name="TObject">对象</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToBytes<TObject>(this TObject obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Flush();
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 将字节数组转换为对象
        /// </summary>
        /// <typeparam name="TObject">需要转换的对象类型</typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static TObject ToObject<TObject>(this byte[] bytes) where TObject: class
        {
            using (var stream = new MemoryStream(bytes, 0, bytes.Length, false))
            {
                var formatter = new BinaryFormatter();
                var data = formatter.Deserialize(stream);
                stream.Flush();
                return data as TObject;
            }
        }

        /// <summary>
        /// 将字节数组转换为对象(struct)
        /// </summary>
        /// <typeparam name="TObject">需要转换的对象类型</typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static TObject ToObjectAsStruct<TObject>(this byte[] bytes) where TObject: struct
        {
            using (var stream = new MemoryStream(bytes, 0, bytes.Length, false))
            {
                var formatter = new BinaryFormatter();
                var data = formatter.Deserialize(stream);
                stream.Flush();
                return (TObject)data;
            }
        }
        #endregion

        #region 结构体和字节数组的转换
        /// <summary>
        /// 结构体转byte数组
        /// </summary>
        /// <param name="structObj">要转换的结构体</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] StructToBytes<TObject>(this TObject structObj) where TObject: struct
        {
            //得到结构体的大小
            var size = Marshal.SizeOf(structObj);
            //创建byte数组
            var bytes = new byte[size];
            //分配结构体大小的内存空间
            var structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }

        /// <summary>
        /// byte数组转结构体
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <param name="type">结构体类型</param>
        /// <returns>转换后的结构体</returns>
        public static object BytesToStuct(this byte[] bytes, Type type)
        {
            //得到结构体的大小
            var size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            var structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            var obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }
        #endregion
    }
}