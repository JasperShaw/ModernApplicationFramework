using System;
using System.Collections.Generic;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    internal interface ILayoutManagementUserInput
    {
        bool TryGetSavedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName);

        bool TryGetApplyLayoutConfirmation(string name, out bool disableConfirmation);

        bool TryGetOverwriteLayoutConfirmation(string name);

        IEnumerable<KeyValuePair<string, WindowLayoutInfo>> ShowManageLayoutsView(IEnumerable<KeyValuePair<string, WindowLayoutInfo>> layoutKeyInfoCollection);

        void ShowApplyLayoutError(string name);

        void ShowSaveLayoutError(string message);
    }
}