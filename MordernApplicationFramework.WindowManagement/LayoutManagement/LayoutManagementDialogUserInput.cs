using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Utilities;
using MordernApplicationFramework.WindowManagement.Properties;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    internal static class LayoutManagementDialogUserInput
    {
        public static bool TryGetSavedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName)
        {
            Validate.IsNotNull(defaultName, nameof(defaultName));
            Validate.IsNotNull(nameValidator, nameof(nameValidator));
            return TextInputDialog.Show(WindowManagement_Resources.SaveLayoutTitle,
                WindowManagement_Resources.SaveLayoutMessage, 100, defaultName, nameValidator, out layoutName);
        }

        public static bool TryGetApplyLayoutConfirmation(string name, out bool disableConfirmation)
        {
            if (MessageDialog.Show(WindowManagement_Resources.ApplyLayoutTitle, string.Format(CultureInfo.CurrentUICulture, WindowManagement_Resources.ApplyLayoutConfirmation, new object[]
            {
                name
            }), MessageDialogCommandSet.OkCancel, WindowManagement_Resources.DisableApplyLayoutWarning, out disableConfirmation) != MessageDialogCommand.Cancel)
                return true;
            disableConfirmation = false;
            return false;
        }

        public static bool TryGetOverwriteLayoutConfirmation(string name)
        {
            return MessageDialog.Show(WindowManagement_Resources.SaveLayoutTitle, string.Format(CultureInfo.CurrentUICulture, WindowManagement_Resources.LayoutOverwriteMessage, new object[]
            {
                name
            }), MessageDialogCommandSet.YesNo) == MessageDialogCommand.Yes;
        }

        public static bool GetRenamedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName)
        {
            Validate.IsNotNull(defaultName, nameof(defaultName));
            Validate.IsNotNull(nameValidator, nameof(nameValidator));
            return TextInputDialog.Show(WindowManagement_Resources.RenameLayoutTitle, WindowManagement_Resources.RenameLayoutMessage,
                100, defaultName, nameValidator, out layoutName);
        }

        public static bool GetReplaceLayoutConfirmation(string name)
        {
            return MessageDialog.Show(WindowManagement_Resources.RenameLayoutTitle, string.Format(CultureInfo.CurrentUICulture, WindowManagement_Resources.LayoutOverwriteMessage, new object[]
            {
                name
            }), MessageDialogCommandSet.YesNo) == MessageDialogCommand.Yes;
        }

        public static bool GetDeleteLayoutConfirmation(string name)
        {
            return MessageDialog.Show(WindowManagement_Resources.DeleteLayoutTitle, string.Format(CultureInfo.CurrentUICulture, WindowManagement_Resources.DeleteLayoutConfirmation, new object[]
            {
                name
            }), MessageDialogCommandSet.YesNo) == MessageDialogCommand.Yes;
        }

        public static IEnumerable<KeyValuePair<string, WindowLayout>> ShowManageLayoutsView(IEnumerable<KeyValuePair<string, WindowLayout>> layoutKeyInfoCollection)
        {
            return ManageLayoutsDialog.Show(layoutKeyInfoCollection);
        }

        public static void ShowApplyLayoutError(string name)
        {
            DisplayError(string.Format(CultureInfo.CurrentUICulture, WindowManagement_Resources.ApplyLayoutFailedMessage, new object[]
            {
                name
            }));
        }

        public static void ShowSaveLayoutError(string message)
        {
            DisplayError(message);
        }

        private static void DisplayError(string message)
        {
            MessageBox.Show(message);
        }
    }
}
