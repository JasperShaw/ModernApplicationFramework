using System;
using System.Collections;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;

namespace ModernApplicationFramework.Core.Comparers
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="IComparer"/> that compared two <see cref="CommandDefinition"/> by its <see cref="CommandDefinition.TrimmedCategoryCommandName"/> name
    /// </summary>
    /// <seealso cref="T:System.Collections.IComparer" />
    public class CategoryCommandTextComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (!(x is CommandDefinition command1) || !(y is CommandDefinition command2))
                throw new ArgumentException("This converter does not support the provided objects");
            var text1 = command1.TrimmedCategoryCommandName;
            var text2 = command2.TrimmedCategoryCommandName;

            if (text1 == null || text2 == null)
                throw new ArgumentException("Couldn't retrieve Name property of the given data sources");
            return string.Compare(text1, text2, StringComparison.Ordinal);
        }
    }
}