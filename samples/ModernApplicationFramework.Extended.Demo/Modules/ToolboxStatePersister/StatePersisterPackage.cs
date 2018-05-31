using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.Extended.Demo.Modules.ToolboxStatePersister
{
    [Export(typeof(IMafPackage))]
    public class StatePersisterPackage : Package.Package
    {
        public override PackageLoadOption LoadOption => PackageLoadOption.OnMainWindowLoaded;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{AD27146F-101A-4C39-A0F0-07C809EA53D7}");
    }
}
