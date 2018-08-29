using System;
using System.Globalization;
using System.Reflection;

namespace ModernApplicationFramework.Utilities
{
    internal static class PrivateErrorHelpers
    {
        internal static Type TrimGenericWrapper(Type type, Type wrapper)
        {
            Type[] genericTypeArguments;
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == wrapper && (genericTypeArguments = type.GenericTypeArguments).Length == 1)
                return genericTypeArguments[0];
            return type;
        }

        internal static string Format(string format, params object[] arguments)
        {
            return string.Format(CultureInfo.CurrentCulture, format, arguments);
        }
    }
}