using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace ModernApplicationFramework.Basics.SettingsManager
{
    public class SettingsValueSerializer
    {
        private static readonly Dictionary<string, Tuple<Type, TypeConverter>> SpecialTypes;

        static SettingsValueSerializer()
        {
            SpecialTypes = new Dictionary<string, Tuple<Type, TypeConverter>>();
            Type[] typeArray = {
                typeof (string),
                typeof (byte),
                typeof (short),
                typeof (int),
                typeof (long),
                typeof (sbyte),
                typeof (ushort),
                typeof (uint),
                typeof (ulong),
                typeof (bool),
                typeof (char),
                typeof (double),
                typeof (float),
                typeof (Guid),
                typeof (Decimal),
                typeof (DateTime),
                typeof (DateTimeOffset),
                typeof (TimeSpan),
                typeof (Size),
                typeof (Point)
            };
            foreach (Type type in typeArray)
                SpecialTypes.Add(type.FullName, Tuple.Create(type, (TypeConverter)null));
        }

        public string Serialize(object obj, Type type)
        {
            if (obj == null)
                return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", '*', "null", '*');
            string str = null;
            if (type.IsEnum)
            {
                type = type.GetEnumUnderlyingType();
                str = ((Enum)obj).ToString("d");
            }
            else if (obj is DateTime)
                str = ((DateTime)obj).ToString("o", CultureInfo.InvariantCulture);
            else if (obj is DateTimeOffset)
                str = ((DateTimeOffset)obj).ToString("o", CultureInfo.InvariantCulture);
            TypeConverter converter = GetConverter(type.FullName);
            if (converter == null)
                throw new InvalidOperationException();
            if (str == null)
                str = converter.ConvertToInvariantString(obj);
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}", '*', type.FullName, '*', str);
        }

        public GetValueResult Deserialize<T>(string s, out T result)
        {
            result = default(T);

            int num = s.IndexOf('*', 1);

            if (num < 0 || num == 1)
                return GetValueResult.Corrupt;

            string typeName = s.Substring(1, num - 1);
            string str = s.Substring(num + 1);
            object obj;
            if (typeName == "null")
                obj = null;
            else
            {
                TypeConverter converter = GetConverter(typeName);
                if (converter == null)
                    return GetValueResult.Corrupt;
                try
                {
                    obj = converter.ConvertFromInvariantString(str);
                }
                catch (Exception)
                {
                    return GetValueResult.Corrupt;
                }
            }
            try
            {
                result = CastWithUnboxing<T>(obj);
            }
            catch (Exception)
            {
                return GetValueResult.IncompatibleType;
            }

            return GetValueResult.Success;
        }

        private static T CastWithUnboxing<T>(object value)
        {
            if (typeof(T).IsEnum)
                return (T)Enum.ToObject(typeof(T), value);
            if (typeof(T) == typeof(bool) && !(value is bool))
            {
                object boxed = value;
                if (TryReboxAnyIntegralTypeAs<ulong>(ref boxed))
                    return (T)(object)((ulong)boxed > 0UL);
            }
            if (value is bool && typeof(T) != typeof(bool))
            {
                object boxed = (bool)value ? 1 : 0;
                if (TryReboxInt32As<T>(ref boxed))
                    value = boxed;
            }
            var flag = TryReboxAnyIntegralTypeAs<T>(ref value) || TryReboxFloatAs<T>(ref value) || TryReboxDoubleAs<T>(ref value) || TryReboxDecimalAs<T>(ref value);
            return (T)value;
        }

        private static bool TryReboxAnyIntegralTypeAs<T>(ref object boxed)
        {
            if (!TryReboxByteAs<T>(ref boxed) && !TryReboxSbyteAs<T>(ref boxed) && !TryReboxInt16As<T>(ref boxed) &&
                !TryReboxUint16As<T>(ref boxed) && (!TryReboxInt32As<T>(ref boxed) && !TryReboxUint32As<T>(ref boxed) &&
                                                    !TryReboxInt64As<T>(ref boxed)))
                return TryReboxUint64As<T>(ref boxed);
            return true;
        }

        private static bool TryReboxByteAs<T>(ref object boxed)
        {
            if (!(boxed is byte))
                return false;
            byte num = (byte)boxed;
            if (typeof(T) == typeof(byte))
                boxed = num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxSbyteAs<T>(ref object boxed)
        {
            if (!(boxed is sbyte))
                return false;
            sbyte num = (sbyte)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxInt16As<T>(ref object boxed)
        {
            if (!(boxed is short))
                return false;
            short num = (short)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxUint16As<T>(ref object boxed)
        {
            if (!(boxed is ushort))
                return false;
            ushort num = (ushort)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxInt32As<T>(ref object boxed)
        {
            if (!(boxed is int))
                return false;
            int num = (int)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxUint32As<T>(ref object boxed)
        {
            if (!(boxed is uint))
                return false;
            uint num = (uint)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxInt64As<T>(ref object boxed)
        {
            if (!(boxed is long))
                return false;
            long num = (long)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = (ulong)num;
            }
            return true;
        }

        private static bool TryReboxUint64As<T>(ref object boxed)
        {
            if (!(boxed is ulong))
                return false;
            ulong num = (ulong)boxed;
            if (typeof(T) == typeof(byte))
                boxed = (byte)num;
            else if (typeof(T) == typeof(sbyte))
                boxed = (sbyte)num;
            else if (typeof(T) == typeof(short))
                boxed = (short)num;
            else if (typeof(T) == typeof(ushort))
                boxed = (ushort)num;
            else if (typeof(T) == typeof(int))
                boxed = (int)num;
            else if (typeof(T) == typeof(uint))
                boxed = (uint)num;
            else if (typeof(T) == typeof(long))
            {
                boxed = (long)num;
            }
            else
            {
                if (!(typeof(T) == typeof(ulong)))
                    return false;
                boxed = num;
            }
            return true;
        }

        private static bool TryReboxFloatAs<T>(ref object boxed)
        {
            if (!(boxed is float))
                return false;
            float num = (float)boxed;
            if (typeof(T) == typeof(float))
                boxed = num;
            else if (typeof(T) == typeof(double))
            {
                boxed = (double)num;
            }
            else
            {
                if (!(typeof(T) == typeof(Decimal)))
                    return false;
                boxed = (Decimal)num;
            }
            return true;
        }

        private static bool TryReboxDoubleAs<T>(ref object boxed)
        {
            if (!(boxed is double))
                return false;
            double num = (double)boxed;
            if (typeof(T) == typeof(float))
                boxed = (float)num;
            else if (typeof(T) == typeof(double))
            {
                boxed = num;
            }
            else
            {
                if (!(typeof(T) == typeof(Decimal)))
                    return false;
                boxed = (Decimal)num;
            }
            return true;
        }

        private static bool TryReboxDecimalAs<T>(ref object boxed)
        {
            if (!(boxed is Decimal))
                return false;
            Decimal num = (Decimal)boxed;
            if (typeof(T) == typeof(float))
                boxed = (float)num;
            else if (typeof(T) == typeof(double))
            {
                boxed = (double)num;
            }
            else
            {
                if (!(typeof(T) == typeof(Decimal)))
                    return false;
                boxed = num;
            }
            return true;
        }


        private static TypeConverter GetConverter(string typeName)
        {
            if (!SpecialTypes.TryGetValue(typeName, out var tuple))
                return null;
            var converter = tuple.Item2;
            if (converter != null)
                return converter;
            var type = tuple.Item1;
            SpecialTypes[typeName] = Tuple.Create(type, converter = TypeDescriptor.GetConverter(type));
            return converter;
        }
    }
}
