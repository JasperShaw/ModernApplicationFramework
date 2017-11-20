using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.WindowManagement.CommandBarLayout
{
    [Export(typeof(IMafPackage))]
    public class CommandBarLayoutPackage : Package
    {
        private readonly ICommandBarSerializer _serializer;
        private readonly CommandBarLayoutSettings _settings;
        public override PackageLoadOption LoadOption => PackageLoadOption.OnMainWindowLoaded;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{016D9005-A120-4E35-8BCE-33CF48250C20}");

        [ImportingConstructor]
        public CommandBarLayoutPackage(ICommandBarSerializer serializer, CommandBarLayoutSettings settings)
        {
            _serializer = serializer;
            _settings = settings;
        }

        public override void Initialize()
        {
            base.Initialize();
            Deserialize();
        }


        private void Deserialize()
        {
            //Disable because startup gets very slow with attached debugger
            if (System.Diagnostics.Debugger.IsAttached)
                return;
            var settings = IoC.Get<CommandBarLayoutSettings>();
            var layout = settings.Layout;
            if (layout == null)
                return;
            if (_serializer.Validate(layout))
                _serializer.Deserialize(layout);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _settings.StoreSettings();
        }
    }
}
