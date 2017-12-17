// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#endregion

namespace TLAuto.Base.Extensions
{
    public static class EnumExtensions
    {
        public static T GetEnumAttribute<T>(this Enum enumType)
        {
            var type = enumType.GetType();
            var fd = type.GetField(enumType.ToString());
            if (fd == null)
            {
                return default(T);
            }
            var attrs = fd.GetCustomAttributes(typeof(T), false);
            return (T)attrs[0];
        }

        public static string GetEnumDescriptionAttribute(this Enum enumType)
        {
            return GetEnumAttribute<DescriptionAttribute>(enumType).Description;
        }

        public static IEnumerable<T> ToEnums<T>()
        {
            var names = Enum.GetNames(typeof(T));
            return names.Select(name => name.ToEnumFromName<T>()).ToList();
        }

        /// <summary>
        /// Convert value to enum.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="value">Value of enum.</param>
        /// <exception cref="System.ArgumentException">If out of range.</exception>
        /// <returns>Return the converted enum.</returns>
        public static T ToEnumFromValue<T>(this object value)
        {
            if (value == null)
            {
                return default(T);
            }
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("type is not a Enum. Type:" + type);
            }
            if (Enum.IsDefined(type, value))
            {
                return (T)Enum.ToObject(type, value);
            }
            throw new ArgumentException("invalid data to be converted to enum");
        }

        /// <summary>
        /// Convert value to enum.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="name">Name of enum.</param>
        /// <exception cref="System.ArgumentException">If out of range.</exception>
        /// <returns>Return the converted enum.</returns>
        public static T ToEnumFromName<T>(this string name)
        {
            if (name == null)
            {
                return default(T);
            }
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("type is not a Enum. Type:" + type);
            }
            if (Enum.IsDefined(type, name))
            {
                return (T)Enum.Parse(type, name);
            }
            throw new ArgumentException("invalid data to be converted to enum");
        }

        public static T ToEnumFromNameWithoutException<T>(this string name)
        {
            if (name == null)
            {
                return default(T);
            }
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("type is not a Enum. Type:" + type);
            }
            if (Enum.IsDefined(type, name))
            {
                return (T)Enum.Parse(type, name);
            }
            return default(T);
        }

        public static bool Validate<T>(this T e)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("type is not a Enum. Type:" + type);
            }

            foreach (T value in Enum.GetValues(type))
            {
                if (e.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Validate<T>(int value)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("type is not a Enum. Type:" + type);
            }

            foreach (T val in Enum.GetValues(type))
            {
                if (val.Equals(Enum.ToObject(type, value)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}