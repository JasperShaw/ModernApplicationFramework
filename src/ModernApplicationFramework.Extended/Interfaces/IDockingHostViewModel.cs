using System;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Controls.DockingHost.ViewModels;

namespace ModernApplicationFramework.Extended.Interfaces
{
    /// <inheritdoc cref="IConductor"/>
    /// <summary>
    /// An <see cref="IConductor"/> that manages the docking behavior of the appliaction
    /// </summary>
    /// <seealso cref="T:Caliburn.Micro.IGuardClose" />
    /// <seealso cref="T:Caliburn.Micro.IDeactivate" />
    /// <seealso cref="T:Caliburn.Micro.IConductor" />
    public interface IDockingHostViewModel : IGuardClose, IDeactivate, IConductor
    {
        /// <summary>
        /// Occurs when then active layout item changed.
        /// </summary>
        event EventHandler<LayoutChangeEventArgs> ActiveLayoutItemChanged;

        /// <summary>
        /// Occurs when the active layout item is about to change.
        /// </summary>
        event EventHandler<LayoutChangeEventArgs> ActiveLayoutItemChanging;

        /// <summary>
        /// The active <see cref="ILayoutItem"/>.
        /// </summary>
        ILayoutItem ActiveItem { get; }

        /// <summary>
        /// The docking host view.
        /// </summary>
        IDockingHost DockingHostView { get; }

        /// <summary>
        /// Observable collection of all <see cref="ILayoutItem"/>s.
        /// </summary>
        IObservableCollection<ILayoutItem> LayoutItems { get; }

        /// <summary>
        /// Observable collection of all <see cref="ITool"/>s.
        /// </summary>
        IObservableCollection<ITool> Tools { get; }

        /// <summary>
        /// Gets or sets the active <see cref="ILayoutItemBase"/>.
        /// </summary>
        ILayoutItemBase ActiveLayoutItemBase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether floating windows will be shown separately in the taskbar.
        /// </summary>
        bool ShowFloatingWindowsInTaskbar { get; set; }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

        /// <summary>
        /// Closes an <see cref="ILayoutItem" />.
        /// </summary>
        /// <param name="layoutItem">The layout item.</param>
        /// <returns>Returns <see langword="false"/> if closing this item was aborted; otherwise <see langword="true"/></returns>
        bool CloseLayoutItem(ILayoutItem layoutItem);


        /// <summary>
        /// Opens an <see cref="ILayoutItem"/>.
        /// </summary>
        /// <param name="model">The layout item</param>
        void OpenLayoutItem(ILayoutItem model);

        /// <summary>
        /// Shows an <see cref="ITool"/>.
        /// </summary>
        /// <typeparam name="TTool">The type of the tool.</typeparam>
        void ShowTool<TTool>() where TTool : ITool;

        /// <summary>
        /// Shows an <see cref="ITool"/>.
        /// </summary>
        /// <param name="tool">The tool model.</param>
        void ShowTool(ITool tool);

        /// <summary>
        /// Hides an <see cref="ITool"/>.
        /// </summary>
        /// <typeparam name="TTool">The type of the tool.</typeparam>
        /// <param name="remove">if set to <see langword="true"/> the tool will be removed.</param>
        void HideTool<TTool>(bool remove) where TTool : ITool;

        /// <summary>
        /// Hides an <see cref="ITool"/>.
        /// </summary>
        /// <param name="tool">The tool model.</param>
        /// <param name="remove">if set to <see langword="true"/> the tool will be removed.</param>
        void HideTool(ITool tool, bool remove);

        /// <summary>
        /// Determines whether a layoutItem already exists.
        /// </summary>
        /// <param name="layoutItem">The layout item to check.</param>
        /// <returns>
        /// <see langword="true"/> if the LayoutItem already exists; <see langword="false"/> otherwise
        /// </returns>
        bool ContainsLayoutItem(ILayoutItem layoutItem);
    }
}