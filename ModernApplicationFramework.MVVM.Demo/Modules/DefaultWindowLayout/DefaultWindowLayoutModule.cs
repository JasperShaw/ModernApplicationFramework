using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Core.ModuleBase;
using ModernApplicationFramework.Extended.Interfaces;
using MordernApplicationFramework.WindowManagement.LayoutManagement;

namespace ModernApplicationFramework.MVVM.Demo.Modules.DefaultWindowLayout
{
    [Export(typeof(IModule))]
    public class DefaultWindowLayoutModule : ModuleBase
    {
        private readonly IDefaultWindowLayoutProvider _defaultWindowLayoutProvider;

        [ImportingConstructor]
        public DefaultWindowLayoutModule(IDefaultWindowLayoutProvider defaultWindowLayoutProvider)
        {
            _defaultWindowLayoutProvider = defaultWindowLayoutProvider;
        }

        public override void PreInitialize()
        {
            _defaultWindowLayoutProvider.SetDefaultLayout("123");
            base.PreInitialize();
        }
    }
}
