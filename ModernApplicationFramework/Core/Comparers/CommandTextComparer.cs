using System.Collections;
using System.Globalization;
using ModernApplicationFramework.Core.Converters;

namespace ModernApplicationFramework.Core.Comparers
{
    public class CommandTextComparer : IComparer
    {
        private static AccessKeyRemovingConverter _accessKeyRemover;

        static CommandTextComparer()
        {
            _accessKeyRemover = new AccessKeyRemovingConverter();
        }

        public int Compare(object x, object y)
        {
            return 1;
        }

        private static string RemoveAccessKey(string input)
        {
            return _accessKeyRemover.Convert(input, typeof(string), null, CultureInfo.CurrentUICulture) as string;
        }
    }
}
