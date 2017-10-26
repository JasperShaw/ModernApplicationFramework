using System;
using System.ComponentModel.Composition;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    [Export(typeof(IDefaultWindowLayoutProvider))]
    public class DefaultWindowLayoutProvider : IDefaultWindowLayoutProvider
    {
        private WindowLayout _windowLayout;


        public WindowLayout GetLayout()
        {
            return _windowLayout;
        }

        public void SetDefaultLayout(string uncompressedPayload)
        {
            _windowLayout = new WindowLayout("default", -1, new Guid().ToString("N"), uncompressedPayload, true);
        }
    }
}
