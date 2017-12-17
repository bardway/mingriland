// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace TLAuto.Base.Extensions
{
    public static class MarshalExtensions
    {
        public static T PtrToStructure<T>(this IntPtr ptr)
        {
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        public static List<T> PtrToStructureList<T>(this IntPtr ptr, int count)
        {
            var ret = new List<T>();
            var size = Marshal.SizeOf(typeof(T));
            for (var i = 0; i < count; i++)
            {
                var currP = ptr + (size * i);
                var data = currP.PtrToStructure<T>();
                ret.Add(data);
            }

            return ret;
        }

        public static bool TryPtrToStructure<T>(this IntPtr ptr, out T result)
        {
            try
            {
                result = ptr.PtrToStructure<T>();
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

        public static bool TryPtrToList<T>(this IntPtr ptr, int count, out List<T> result)
        {
            try
            {
                result = ptr.PtrToStructureList<T>(count);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}