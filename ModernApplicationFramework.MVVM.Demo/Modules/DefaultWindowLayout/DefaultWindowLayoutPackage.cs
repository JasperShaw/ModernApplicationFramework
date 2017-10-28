using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Core.Package;
using MordernApplicationFramework.WindowManagement.LayoutManagement;

namespace ModernApplicationFramework.MVVM.Demo.Modules.DefaultWindowLayout
{
    [Export(typeof(IMafPackage))]
    public class DefaultWindowLayoutPackage : Package
    {
        private IDefaultWindowLayoutProvider _defaultWindowLayoutProvider;

        public override Guid Id => new Guid("{DC9C672E-A0EB-4D77-A825-C8690DD115C1}");

        [ImportingConstructor]
        public DefaultWindowLayoutPackage(IDefaultWindowLayoutProvider defaultWindowLayoutProvider)
        {
            _defaultWindowLayoutProvider = defaultWindowLayoutProvider;
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
