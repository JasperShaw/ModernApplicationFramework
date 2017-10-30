using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.WindowManagement.LayoutManagement
{
    internal interface ILayoutManagerInternal
    {
        void SaveWindowLayoutInternal(string layoutName, bool hadNameConflict);

        void SaveWindowLayoutInternal(string layoutName, string layoutPayload, bool hadNameConflict);

        void ManageWindowLayoutsInternal(Func<IEnumerable<KeyValuePair<string, WindowLayout>>, IEnumerable<KeyValuePair<string, WindowLayout>>> layoutTransformation);

        void ApplyWindowLayoutInternal(int index);
    }
}