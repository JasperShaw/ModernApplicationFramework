using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Utilities;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    internal static class LayoutManagementDialogUserInput
    {
        public static bool TryGetSavedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName)
        {
            Validate.IsNotNull(defaultName, "defaultName");
            Validate.IsNotNull(nameValidator, "nameValidator");
            return TextInputDialog.Show("Save", "SaveMessage", 100, defaultName, nameValidator, out layoutName);
        }

        public static bool TryGetApplyLayoutConfirmation(string name, out bool disableConfirmation)
        {
            if (MessageDialog.Show("Apply", string.Format(CultureInfo.CurrentUICulture, "ApplyConform {0}", new object[]
            {
                name
            }), MessageDialogCommandSet.OkCancel, "Not Showing", out disableConfirmation) != MessageDialogCommand.Cancel)
                return true;
            disableConfirmation = false;
            return false;
        }

        public static bool TryGetOverwriteLayoutConfirmation(string name)
        {
            return MessageDialog.Show("Save", string.Format(CultureInfo.CurrentUICulture, "Override {0}", new object[]
            {
                name
            }), MessageDialogCommandSet.YesNo) == MessageDialogCommand.Yes;
        }

        public static bool GetRenamedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName)
        {
            Validate.IsNotNull(defaultName, "defaultName");
            Validate.IsNotNull(nameValidator, "nameValidator");
            return TextInputDialog.Show("Rename",
                "Rename", 100, defaultName, nameValidator, out layoutName);
        }

        public static bool GetReplaceLayoutConfirmation(string name)
        {
            return MessageDialog.Show("Rename", string.Format(
                       CultureInfo.CurrentUICulture,
                       "Rename"), MessageDialogCommandSet.YesNo) == MessageDialogCommand.Yes;
        }

        public static bool GetDeleteLayoutConfirmation(string name)
        {
            return MessageDialog.Show("Rename", string.Format(
                       CultureInfo.CurrentUICulture,
                       "Rename"), MessageDialogCommandSet.YesNo) == MessageDialogCommand.Yes;
        }

        public static void ShowManageLayoutsView(IEnumerable<KeyValuePair<string, WindowLayout>> layoutKeyInfoCollection)
        {
            ManageLayoutsDialog.Show(layoutKeyInfoCollection);
        }

        public static void ShowApplyLayoutError(string name)
        {
            DisplayError(string.Format(CultureInfo.CurrentUICulture, "Could not apply {0}", new object[]
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
