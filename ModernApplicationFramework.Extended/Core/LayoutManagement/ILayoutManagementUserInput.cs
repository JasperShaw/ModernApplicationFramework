using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Extended.Core.LayoutManagement
{
    internal interface ILayoutManagementUserInput
    {
        bool TryGetSavedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName);

        bool TryGetApplyLayoutConfirmation(string name, out bool disableConfirmation);

        bool TryGetOverwriteLayoutConfirmation(string name);

        bool GetRenamedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName);

        bool GetReplaceLayoutConfirmation(string name);

        bool GetDeleteLayoutConfirmation(string name);

        IEnumerable<KeyValuePair<string, WindowLayoutInfo>> ShowManageLayoutsView(IEnumerable<KeyValuePair<string, WindowLayoutInfo>> layoutKeyInfoCollection);

        void ShowApplyLayoutError(string name);

        void ShowSaveLayoutError(string message);
    }
}