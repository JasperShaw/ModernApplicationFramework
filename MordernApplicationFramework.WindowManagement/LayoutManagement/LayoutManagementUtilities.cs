using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ModernApplicationFramework.Utilities;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    internal static class LayoutManagementUtilities
    {
        internal static string NormalizeName(string name)
        {
            return name.Trim();
        }

        internal static bool IsUniqueName(string name, IEnumerable<string> existingNames)
        {
            return IsUniqueName(name, existingNames, out var _);
        }

        internal static bool IsUniqueName(string name, IEnumerable<string> existingNames, out int conflictingIndex)
        {
            int num = 0;
            foreach (var existingName in existingNames)
            {
                if (string.Equals(existingName, name, StringComparison.CurrentCultureIgnoreCase))
                {
                    conflictingIndex = num;
                    return false;
                }
                ++num;
            }
            conflictingIndex = -1;
            return true;
        }

        internal static bool IsUniqueName(string name, IWindowLayoutStore store)
        {
            return IsUniqueName(name, EnumerateLayoutKeyInfo(store).Select(info => info.Value.Name));
        }

        internal static IEnumerable<KeyValuePair<string, WindowLayoutInfo>> EnumerateLayoutKeyInfo(IWindowLayoutStore store)
        {
            Validate.IsNotNull(store, "store");
            var keyValuePairList = new List<KeyValuePair<string, WindowLayoutInfo>>();
            var layoutCount = store.GetLayoutCount();
            for (var index = 0; index < layoutCount; ++index)
                keyValuePairList.Add(store.GetLayoutAt(index));
            return keyValuePairList;
        }

        internal static string GenerateKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static Stream ConvertLayoutPayloadToStream(string payload)
        {
            var bytes = payload.Split('-').Select(s => Convert.ToByte(s, 10)).ToArray();
            return new MemoryStream(bytes);
        }

        public static string ConvertLayoutStreamToString(IEnumerable<byte> data)
        {
            var sb = new StringBuilder();
            foreach (var b in data)
            {
                sb.Append(b);
                sb.Append('-');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}
