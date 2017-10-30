using System.Collections.Generic;

namespace ModernApplicationFramework.WindowManagement.LayoutManagement
{
    internal interface IWindowLayoutStore
    {
        KeyValuePair<string, WindowLayout> GetLayoutAt(int index);

        string GetLayoutDataAt(int index);

        int GetLayoutCount();

        string SaveLayout(string layoutName, string data);

        void UpdateStore(IEnumerable<KeyValuePair<string, WindowLayout>> newLayouts);
    }
}