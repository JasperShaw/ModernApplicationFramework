using System.Collections.Generic;

namespace ModernApplicationFramework.Extended.Core.LayoutManagement
{
    internal interface IWindowLayoutStore
    {
        KeyValuePair<string, WindowLayoutInfo> GetLayoutAt(int index);

        string GetLayoutDataAt(int index);

        int GetLayoutCount();

        string SaveLayout(string layoutName, string data);

        void UpdateLayouts(IEnumerable<KeyValuePair<string, WindowLayoutInfo>> keyInfoCollection);
    }
}