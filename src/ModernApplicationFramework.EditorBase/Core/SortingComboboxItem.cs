using System;
using System.ComponentModel;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;

namespace ModernApplicationFramework.EditorBase.Core
{
    public class SortingComboboxItem : TextCommandBarItemDefinition, ISortingComboboxItem
    {
        public delegate int CompareTemplates(IExtensionDefinition source, IExtensionDefinition target);

        private readonly CompareTemplates _compareTemplates;

        public Func<object, object, int> Comparer;

        public SortingComboboxItem(string text, ListSortDirection direction, Func<IExtensionDefinition, IExtensionDefinition, int> sortLogic) : base(text)
        {
            var sortDir = direction == ListSortDirection.Ascending ? 1 : -1;
            _compareTemplates = (source, target) => sortLogic(source, target) * sortDir;
        }

        public int Compare(object x, object y)
        {
            return _compareTemplates((IExtensionDefinition) x, (IExtensionDefinition) y);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}