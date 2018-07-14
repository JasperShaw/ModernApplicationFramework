using System;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    /// <inheritdoc cref="ColumnInformation" />
    /// <summary>
    /// Custom <see cref="T:ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog.ColumnInformation" /> that holds a comparer function
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog.ColumnInformation" />
    /// <seealso cref="T:ModernApplicationFramework.Modules.Toolbox.Interfaces.ICustomSortColumn" />
    public class CustomSortColumnDataSource : ColumnInformation, ICustomSortColumn
    {
        private readonly Func<string, string, int> _comparer;

        public CustomSortColumnDataSource(string propertyName, string localizedName,
            Func<string, string, int> comparer) :
            base(propertyName, localizedName)
        {
            _comparer = comparer;
        }

        /// <summary>
        /// Compares the two inputs.
        /// </summary>
        /// <param name="first">The first input.</param>
        /// <param name="second">The second input.</param>
        /// <returns>Comparer result</returns>
        public int Compare(string first, string second)
        {
            return _comparer(first, second);
        }
    }
}