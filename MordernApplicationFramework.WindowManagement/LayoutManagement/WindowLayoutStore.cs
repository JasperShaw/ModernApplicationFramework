using System.Collections.Generic;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    internal class WindowLayoutStore : IWindowLayoutStore
    {
        public KeyValuePair<string, WindowLayoutInfo> GetLayoutAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public string GetLayoutDataAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public int GetLayoutCount()
        {
            return 0;
        }

        public string SaveLayout(string layoutName, string data)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateLayouts(IEnumerable<KeyValuePair<string, WindowLayoutInfo>> keyInfoCollection)
        {
            throw new System.NotImplementedException();
        }
    }
}