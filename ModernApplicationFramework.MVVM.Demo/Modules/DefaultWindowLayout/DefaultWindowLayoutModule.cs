using System.ComponentModel.Composition;
using System.IO;
using ModernApplicationFramework.Extended.Core.ModuleBase;
using ModernApplicationFramework.Extended.Interfaces;
using MordernApplicationFramework.WindowManagement.LayoutManagement;
using MordernApplicationFramework.WindowManagement.LayoutState;

namespace ModernApplicationFramework.MVVM.Demo.Modules.DefaultWindowLayout
{
    [Export(typeof(IModule))]
    public class DefaultWindowLayoutModule : ModuleBase
    {
        private readonly IDefaultWindowLayoutProvider _defaultWindowLayoutProvider;
        private readonly ILayoutItemStatePersister _statePersister;

        [ImportingConstructor]
        public DefaultWindowLayoutModule(IDefaultWindowLayoutProvider defaultWindowLayoutProvider, 
            ILayoutItemStatePersister statePersister)
        {
            _defaultWindowLayoutProvider = defaultWindowLayoutProvider;
            _statePersister = statePersister;
        }

        public override void PreInitialize()
        {
            var file= Path.Combine(_statePersister.ApplicationStateDirectory, "default.winprf");
            if (!File.Exists(file))
                return;
            var payload = _statePersister.FileToPayloadData(file);
            _defaultWindowLayoutProvider.SetDefaultLayout(payload);
            base.PreInitialize();
        }
    }
}
