using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Modules.Toolbox.Resources;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog.Internal
{
    [Export(typeof(IChooseItemsPageInfo))]
    internal class AllToolboxItemsPageInfo : IChooseItemsPageInfo
    {
        public IEnumerable<ColumnInformation> Columns => new List<ColumnInformation>
        {
            new ColumnInformation(nameof(ItemDataSource.Name), ChooseItemsDialogResources.AllToolboxItemsPageInfo_ColumnName),
            new ColumnInformation(nameof(ItemDataSource.Namespace), ChooseItemsDialogResources.AllToolboxItemsPageInfo_ColumnNamespace),
            new ColumnInformation(nameof(ItemDataSource.AssemblyName), ChooseItemsDialogResources.AllToolboxItemsPageInfo_ColumnAssembly),
            new CustomSortColumnDataSource(nameof(ItemDataSource.AssemblyVersion), ChooseItemsDialogResources.AllToolboxItemsPageInfo_ColumnVersion, VersionComare)
        };

        public Guid Id => new Guid("{EFC137BE-4E39-4A7E-8D6E-7310B9CA40B9}");
        public IItemDataFactory ItemFactory => DefaultItemDataFactory.Default;
        public string Name => ChooseItemsDialogResources.AllToolboxItemsPageInfo_Name;
        public int Order => 0;

        public Predicate<ToolboxItemDefinitionBase> Selector => definition => true;
        public bool ShowAssembly { get; } = true;
        public bool ShowVersion { get; } = true;

        private static int VersionComare(string first, string second)
        {
            if (Version.TryParse(first, out var version1) && Version.TryParse(second, out var version2))
                return version1.CompareTo(version2);
            return string.Compare(first, second, StringComparison.CurrentCulture);
        }
    }
}