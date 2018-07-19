using System;
using System.Collections.Generic;
using ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    /// <summary>
    /// Definition data to create toolbox item pages in the 'Choose Toolbox Items' Dialog
    /// </summary>
    public interface IChooseItemsPageInfo
    {
        /// <summary>
        /// The column informations for this page
        /// </summary>
        IEnumerable<ColumnInformation> Columns { get; }

        /// <summary>
        /// The ID of the page
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Instance of an item factory
        /// </summary>
        IItemDataFactory ItemFactory { get; }

        /// <summary>
        /// Localized name of the page
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Sorting order of the page
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Selector to filter <see cref="ToolboxItemDefinitionBase"/> instances
        /// </summary>
        Predicate<ToolboxItemDefinitionBase> Selector { get; }

        /// <summary>
        /// Gets a value indicating whether the page shall display assembly information.
        /// </summary>
        bool ShowAssembly { get; }

        /// <summary>
        /// Gets a value indicating whether the page shall display assembly version information.
        /// </summary>
        bool ShowVersion { get; }
    }
}