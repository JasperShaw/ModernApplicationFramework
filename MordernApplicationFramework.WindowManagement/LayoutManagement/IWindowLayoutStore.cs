using System.Collections.Generic;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    internal interface IWindowLayoutStore
    {
        KeyValuePair<string, WindowLayout> GetLayoutAt(int index);

        string GetLayoutDataAt(int index);

        int GetLayoutCount();

        string SaveLayout(string layoutName, string data);

        void UpdateStore();
    }
}