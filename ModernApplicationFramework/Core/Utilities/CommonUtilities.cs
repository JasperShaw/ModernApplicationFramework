using System;
using System.Reflection;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class CommonUtilities
    {
        public static Uri MakePackUri(Assembly assembly, string path)
        {
            return new Uri(string.Format("pack://application:,,,/{0};component/{1}", (object)assembly.GetName().Name, (object)path), UriKind.Absolute);
        }
    }
}
