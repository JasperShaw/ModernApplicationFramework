using System;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Special <see cref="T:ModernApplicationFramework.Modules.Toolbox.Interfaces.IToolboxNode" /> that is used for toolbox items
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Modules.Toolbox.Interfaces.IToolboxNode" />
    public interface IToolboxItem : IToolboxNode
    {
        /// <summary>
        /// The data definition.
        /// </summary>
        ToolboxItemDefinitionBase DataSource { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is enabled.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is visible.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// The original parent category.
        /// </summary>
        /// <remarks>
        /// Used for backups
        /// </remarks>
        IToolboxCategory OriginalParent { get; }

        /// <summary>
        /// The current parent category.
        /// </summary>
        IToolboxCategory Parent { get; set; }

        /// <summary>
        /// Evaluates the enabled state of the item.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>Returns <see langword="true"/> is items shall be enabled; <see langword="false"/> Special <see cref="IToolboxNode"/> otherwise.</returns>
        bool EvaluateEnabled(Type targetType);
    }
}