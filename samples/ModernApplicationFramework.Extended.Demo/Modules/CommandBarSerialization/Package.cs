using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.Extended.Demo.Modules.CommandBarSerialization
{
    [Export(typeof(IMafPackage))]
    public class Package : Extended.Package.Package
    {
        private readonly ICommandBarSerializer _serializer;
        public override PackageLoadOption LoadOption => PackageLoadOption.OnMainWindowLoaded;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{016D9005-A120-4E35-8BCE-33CF48250C20}");

        [ImportingConstructor]
        public Package(ICommandBarSerializer serializer)
        {
            _serializer = serializer;
        }

        public override void Initialize()
        {
            base.Initialize();
            _serializer.Serialize();
        }
    }
}
