using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.WindowManagement.LayoutManagement
{
    internal interface ILayoutManagementUserInput
    {
        bool TryGetSavedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName);

        bool TryGetApplyLayoutConfirmation(string name, out bool disableConfirmation);

        bool TryGetOverwriteLayoutConfirmation(string name);

        IEnumerable<KeyValuePair<string, WindowLayout>> ShowManageLayoutsView(IEnumerable<KeyValuePair<string, WindowLayout>> layoutKeyInfoCollection);

        void ShowApplyLayoutError(string name);

        void ShowSaveLayoutError(string message);
    }
}