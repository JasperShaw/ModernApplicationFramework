using System;
using System.Collections;
using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.Core.Converters.AccessKey;

namespace ModernApplicationFramework.Core.Comparers
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="IComparer"/> that compared two <see cref="CommandBarCategory"/> by name
    /// </summary>
    /// <seealso cref="T:System.Collections.IComparer" />
    public class CategoryTextComparer : IComparer
    {
        private static readonly AccessKeyRemovingConverter AccessKeyRemover;

        static CategoryTextComparer()
        {
            AccessKeyRemover = new AccessKeyRemovingConverter();
        }

        public int Compare(object x, object y)
        {
            if (!(x is CommandBarCategory category1) || !(y is CommandBarCategory category2))
                throw new ArgumentException("This converter does not support the provided objects");
            var text1 = category1.Name;
            var text2 = category2.Name;

            if (text1 == null || text2 == null)
                throw new ArgumentException("Couldn't retrieve Name property of the given data sources");
            return string.Compare(RemoveAccessKey(text1), RemoveAccessKey(text2), StringComparison.Ordinal);
        }

        private static string RemoveAccessKey(string input)
        {
            return AccessKeyRemover.Convert(input, typeof(string), null, CultureInfo.CurrentUICulture) as string;
        }
    }
}
