using System;
using System.ComponentModel.Composition;
using System.IO;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.WindowManagement
{
    [Export(typeof(IMafPackage))]
    public class CommandBarLayoutPackage : Package
    {
        private readonly ICommandBarSerializer _serializer;
        public override PackageLoadOption LoadOption => PackageLoadOption.OnMainWindowLoaded;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{016D9005-A120-4E35-8BCE-33CF48250C20}");

        [ImportingConstructor]
        public CommandBarLayoutPackage(ICommandBarSerializer serializer)
        {
            _serializer = serializer;
        }

        public override void Initialize()
        {
            base.Initialize();
            Deserialize();
        }


        private void Deserialize()
        {
            using (var stream = new FileStream(@"C:\Test\CommandBar.xml", FileMode.Open, FileAccess.Read))
            {
                if (_serializer.Validate(stream))
                {
                    stream.Seek(0L, SeekOrigin.Begin);
                    _serializer.Deserialize(stream);
                }
            }
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            using (var stream = new FileStream(@"C:\Test\CommandBar.xml", FileMode.Create, FileAccess.Write))
            {
                _serializer.Serialize(stream);
            }
        }
    }
}
