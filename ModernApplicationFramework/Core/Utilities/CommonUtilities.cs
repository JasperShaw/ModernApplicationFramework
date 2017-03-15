using System;
using System.Reflection;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class CommonUtilities
    {
        public static Uri MakePackUri(Assembly assembly, string path)
        {
            return new Uri($"pack://application:,,,/{assembly.GetName().Name};component/{path}", UriKind.Absolute);
        }
    }
}
