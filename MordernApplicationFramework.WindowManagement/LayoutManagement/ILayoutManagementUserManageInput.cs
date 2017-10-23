using System;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    internal interface ILayoutManagementUserManageInput
    {
        bool GetRenamedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName);

        bool GetReplaceLayoutConfirmation(string name);

        bool GetDeleteLayoutConfirmation(string name);
    }
}