using System;

namespace ModernApplicationFramework.WindowManagement.LayoutManagement
{
    internal interface ILayoutManagementUserManageInput
    {
        bool GetRenamedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName);

        bool GetReplaceLayoutConfirmation(string name);

        bool GetDeleteLayoutConfirmation(string name);
    }
}