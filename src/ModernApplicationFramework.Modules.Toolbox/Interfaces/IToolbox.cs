using System.Collections.Generic;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Toolbox tool window
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Extended.Interfaces.ITool" />
    public interface IToolbox : ITool
    {
        /// <summary>
        /// Copy of the current layout state of the toolbox.
        /// </summary>
        IReadOnlyCollection<IToolboxCategory> CurrentLayout { get; }

        /// <summary>
        /// The selected category.
        /// </summary>
        IToolboxCategory SelectedCategory { get; }

        /// <summary>
        /// Gets or selects an <see cref="IToolboxNode"/>.
        /// </summary>
        IToolboxNode SelectedNode { get; set; }

        /// <summary>
        /// Enables or disables visibility to all toolbox nodes
        /// </summary>
        bool ShowAllItems { get; set; }

        /// <summary>
        /// Refreshes the view.
        /// </summary>
        void RefreshView();
    }
}