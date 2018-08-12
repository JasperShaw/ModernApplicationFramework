using System;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Native.Platform.Structs;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// This interface provides access to basic windowing functionality.
    /// </summary>
    public interface IMafUIShell
    {
        /// <summary>
        /// Returns the HWND that can be used to parent modal dialogs.
        /// </summary>
        /// <param name="phwnd">Pointer to a window handle that can be used to parent modal dialogs.</param>
        /// <returns>If the method succeeds, it returns 0. If it fails, it returns an error code.</returns>
        int GetDialogOwnerHwnd(out IntPtr phwnd);

        /// <summary>
        /// Enables or disables a frame's modeless dialog box.
        /// </summary>
        /// <param name="fEnable">1 when exiting a modal state. 0 when entering a modal state.</param>
        /// <returns>If the method succeeds, it returns 0. If it fails, it returns an error code.</returns>
        IntPtr EnableModeless(int fEnable);

        /// <summary>
        /// Centers the provided dialog box HWND on the parent HWND (if provided), or on the main window.
        /// </summary>
        /// <param name="hwndDialog">Specifies HWND dialog.</param>
        /// <param name="hwndParent">Specifies HWND parent.</param>
        /// <returns>If the method succeeds, it returns 0. If it fails, it returns an error code.</returns>
        int CenterDialogOnWindow(IntPtr hwndDialog, IntPtr hwndParent);

        /// <summary>
        /// Returns the name of the application.
        /// </summary>
        /// <param name="pbstrAppName">Pointer to the name of the application</param>
        /// <returns>If the method succeeds, it returns 0. If it fails, it returns an error code.</returns>
        int GetAppName(out string pbstrAppName);
        void EnableModeless(int v, IntPtr handel);

        /// <summary>
        /// Shows a context menu at specified position.
        /// </summary>
        /// <param name="position">The absolte position.</param>
        /// <param name="contextMenu">The Id of the <see cref="ContextMenuDefinition"/></param>
        void ShowContextMenu(System.Windows.Point position, Guid contextMenu);
    }
}
