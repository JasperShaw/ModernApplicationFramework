using System;
using System.Collections.Generic;

namespace MordernApplicationFramework.WindowManagement
{
    public interface ILayoutManagerInternal
    {
        void SaveWindowLayoutInternal(string layoutName, bool hadNameConflict);

        void SaveWindowLayoutInternal(string layoutName, string layoutPayload, bool hadNameConflict);

        void ManageWindowLayoutsInternal(
            Func<IEnumerable<KeyValuePair<string, WindowLayoutInfo>>,
                IEnumerable<KeyValuePair<string, WindowLayoutInfo>>> layoutTransformation);

        void ApplyWindowLayoutInternal(int index);
    }
}