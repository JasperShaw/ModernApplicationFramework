using System;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public class CustomSortColumnDataSource : ColumnInformation, ICustomSortColumn
    {
        private readonly Func<string, string, int> _comparer;

        public CustomSortColumnDataSource(string propertyName, string localizedName,
            Func<string, string, int> comparer) :
            base(propertyName, localizedName)
        {
            _comparer = comparer;
        }

        public int Compare(string first, string second)
        {
            return _comparer(first, second);
        }
    }
}