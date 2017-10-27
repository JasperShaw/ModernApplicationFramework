using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Core.ModuleBase;
using ModernApplicationFramework.Extended.Interfaces;

namespace MordernApplicationFramework.WindowManagement
{
    [Export(typeof(IModule))]
    public sealed class LayoutManagementModule : ModuleBase
    {
        private LayoutManagementService _layoutManagementService;

        public override void PreInitialize()
        {
            _layoutManagementService = new LayoutManagementService();
            base.PreInitialize();
        }

        public override void Initialize()
        {
            _layoutManagementService.Initialize();
        }

        protected override void DisposeManagedResources()
        {
            _layoutManagementService.Dispose();
            base.DisposeManagedResources();
        }
    }
}
