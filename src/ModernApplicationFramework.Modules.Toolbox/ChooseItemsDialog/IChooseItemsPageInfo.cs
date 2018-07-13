using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public interface IChooseItemsPageInfo
    {
        string Name { get; }

        Guid Id { get; }

        int Order { get; }

        bool ShowAssembly { get; }

        bool ShowVersion { get; }

        IEnumerable<ColumnInformation> Columns { get; }

        Predicate<ToolboxItemDefinitionBase> Selector { get; }

        IItemDataFactory ItemFactory { get; }
    }

    [Export(typeof(IChooseItemsPageInfo))]
    internal class AllToolboxItemsPageInfo : IChooseItemsPageInfo
    {
        public string Name => "All Items";
        public Guid Id => new Guid("{EFC137BE-4E39-4A7E-8D6E-7310B9CA40B9}");
        public int Order => 0;
        public bool ShowAssembly { get; } = true;
        public bool ShowVersion { get; } = true;

        public IEnumerable<ColumnInformation> Columns => new List<ColumnInformation>
        {
            new ColumnInformation(nameof(ItemDataSource.Name), "Name"),
            new ColumnInformation(nameof(ItemDataSource.Namespace), "Namespace"),
            new ColumnInformation(nameof(ItemDataSource.AssemblyName), "Assembly"),
            new CustomSortColumnDataSource(nameof(ItemDataSource.AssemblyVersion), "Version", VersionComare)
        };

        public Predicate<ToolboxItemDefinitionBase> Selector => definition => true;
        public IItemDataFactory ItemFactory => DefaultItemDataFactory.Default;

        private int VersionComare(string first, string second)
        {
            if (Version.TryParse(first, out var version1) && Version.TryParse(second, out var version2))
                return version1.CompareTo(version2);
            return string.Compare(first, second, StringComparison.CurrentCulture);
        }
    }
}
