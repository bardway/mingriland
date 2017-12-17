// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Base.Extensions
{
    public static class CharExtensions
    {
        /// <summary>
        /// 根据长度自动补位(从第一位开始)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataSize"></param>
        /// <returns></returns>
        public static char[] ConvertCharsToFirstLength(this char[] data, int dataSize)
        {
            if (data.Length == dataSize)
            {
                return data;
            }
            var chars = new char[dataSize];
            for (var i = 0; i < dataSize; i++)
            {
                if (data.Length >= i + 1)
                {
                    chars[i] = data[i];
                }
                else
                {
                    chars[i] = '0';
                }
            }
            return chars;
        }

        /// <summary>
        /// 插入指定位置
        /// </summary>
        /// <param name="data"></param>
        /// <param name="insertIndex"></param>
        /// <param name="insertChar"></param>
        /// <returns></returns>
        public static char[] ConvertCharsToInsertLength(this char[] data, int insertIndex, char insertChar)
        {
            var chars = new char[data.Length + 1];
            for (var i = 0; i < chars.Length; i++)
            {
                if (i == insertIndex)
                {
                    chars[i] = insertChar;
                }
                else
                {
                    if (i < data.Length)
                    {
                        chars[i] = data[i];
                    }
                    else
                    {
                        chars[i] = data[i - 1];
                    }
                }
            }
            return chars;
        }

        /// <summary>
        /// 删除指定位置
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="deleteLength"></param>
        /// <returns></returns>
        public static char[] ConvertCharsToDeleteLength(this char[] data, int startIndex, int deleteLength)
        {
            var allLength = data.Length - deleteLength;
            var chars = new char[allLength];
            var index = 0;
            for (var i = 0; i < data.Length; i++)
            {
                if ((i < startIndex) || (i > (startIndex + deleteLength) - 1))
                {
                    chars[index] = data[i];
                    index++;
                }
            }
            return chars;
        }

        /// <summary>
        /// 根据长度自动补位(从最后一位开始)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataSize"></param>
        /// <returns></returns>
        public static char[] ConvertCharsToEndLength(this char[] data, int dataSize)
        {
            if (data.Length == dataSize)
            {
                return data;
            }
            var endLength = data.Length;
            var chars = new char[dataSize];
            for (var i = dataSize - 1; i >= 0; i--)
            {
                if (endLength >= i + 1)
                {
                    chars[i] = data[i];
                }
                else
                {
                    chars[i] = '0';
                }
            }
            return chars;
        }
    }
}