using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Core.Package;

namespace MordernApplicationFramework.WindowManagement
{
    [Export(typeof(IMafPackage))]
    public class LayoutManagementPackage : Package
    {
        private LayoutManagementService _layoutManagementService;

        public override Guid Id => new Guid("{BC313FD7-C2E3-4188-9C82-CFD5BEF6A822}");

        public override void Initialize()
        {
            _layoutManagementService = new LayoutManagementService();
            _layoutManagementService.Initialize();
        }

        protected override void DisposeManagedResources()
        {
            _layoutManagementService.Dispose();
            base.DisposeManagedResources();
        }
    }
}
