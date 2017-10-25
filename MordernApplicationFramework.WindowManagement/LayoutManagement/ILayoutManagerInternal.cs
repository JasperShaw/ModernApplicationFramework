using System;
using System.Collections.Generic;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    public interface ILayoutManagerInternal
    {
        void SaveWindowLayoutInternal(string layoutName, bool hadNameConflict);

        void SaveWindowLayoutInternal(string layoutName, string layoutPayload, bool hadNameConflict);

        void ManageWindowLayoutsInternal(Func<IEnumerable<KeyValuePair<string, WindowLayout>>, IEnumerable<KeyValuePair<string, WindowLayout>>> layoutTransformation);

        void ApplyWindowLayoutInternal(int index);
    }
}