using System;
using Caliburn.Micro;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Special <see cref="IToolboxNode"/> that is used for toolbox categories
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Modules.Toolbox.Interfaces.IToolboxNode" />
    public interface IToolboxCategory : IToolboxNode
    {
        /// <summary>
        /// The category definition.
        /// </summary>
        ToolboxCategoryDefinition DataSource { get; }

        /// <summary>
        /// Value indicating whether this category has enabled items.
        /// </summary>
        bool HasEnabledItems { get; }

        /// <summary>
        /// Value indicating whether this category has items.
        /// </summary>
        bool HasItems { get; }

        /// <summary>
        /// Value indicating whether this category has visible items.
        /// </summary>
        bool HasVisibleItems { get; }

        /// <summary>
        /// Gets or sets the visibility of this category.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        IObservableCollection<IToolboxItem> Items { get; set; }

        /// <summary>
        /// Invalidates the visual state of this category.
        /// </summary>
        void InvalidateState();

        /// <summary>
        /// Refreshes this category and it's items and validates it's state based on the specified target type.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="forceVisibile">if set to <see langword="true"/> items will be forced to show.</param>
        void Refresh(Type targetType, bool forceVisibile = false);

        /// <summary>
        /// Removes an item from this category.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Reutrns <see langword="true"/> if the removing was successful; <see langword="false"/> otherwise</returns>
        bool RemoveItem(IToolboxItem item);
    }
}