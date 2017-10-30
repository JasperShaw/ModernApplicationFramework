using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Core.Package;

namespace ModernApplicationFramework.WindowManagement
{
    [Export(typeof(IMafPackage))]
    public sealed class LayoutManagementPackage : Package
    {
        private LayoutManagementService _layoutManagementService;

        public override PackageLoadOption LoadOption => PackageLoadOption.OnApplicationStart;
        public override PackageCloseOption CloseOption => PackageCloseOption.PreviewApplicationClosed;
        public override Guid Id => new Guid("{BC313FD7-C2E3-4188-9C82-CFD5BEF6A822}");

        public static LayoutManagementPackage Instance { get; private set; }

        public LayoutManagementPackage()
        {
            Instance = this;
        }

        public ILayoutManagementService LayoutManagementSystem { get; private set; }

        public override void Initialize()
        {
            _layoutManagementService = new LayoutManagementService();
            _layoutManagementService.Initialize();
            LayoutManagementSystem = _layoutManagementService;
        }

        protected override void DisposeManagedResources()
        {
            _layoutManagementService.Dispose();
            base.DisposeManagedResources();
        }
    }
}
