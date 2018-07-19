using System;
using System.ComponentModel;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// A node that is used by the toolbox tree view
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public interface IToolboxNode : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when the node was created.
        /// </summary>
        event EventHandler Created;

        /// <summary>
        /// Occurs when the creation of the node was cancelled.
        /// </summary>
        event EventHandler CreatedCancelled;

        /// <summary>
        /// Name during renaming the node
        /// </summary>
        string EditingName { get; set; }


        /// <summary>
        /// The ID of the node
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Value indicating whether this node is flagged custom.
        /// </summary>
        bool IsCustom { get; }

        /// <summary>
        /// Value indicating whether this node is expanded.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Value indicating whether this node is in rename mode.
        /// </summary>
        bool IsInRenameMode { get; set; }

        /// <summary>
        /// Value indicating whether this node's name was modified.
        /// </summary>
        bool IsNameModified { get; }

        /// <summary>
        /// Value indicating whether this node was just created.
        /// </summary>
        bool IsNewlyCreated { get; }

        /// <summary>
        /// Localized name of the node.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Apply the <see cref="EditingName"/>.
        /// </summary>
        void CommitRename();

        /// <summary>
        /// Enters the rename mode.
        /// </summary>
        void EnterRenameMode();

        /// <summary>
        /// Exits the rename mode.
        /// </summary>
        void ExitRenameMode();

        /// <summary>
        /// Determines whether the <see cref="EditingName"/> is valid for commit.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        bool IsRenameValid(out string errorMessage);

        /// <summary>
        /// Resets this node's state.
        /// </summary>
        void Reset();
    }
}