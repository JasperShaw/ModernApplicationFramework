using System;
using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.Extended.Demo.Modules.UIContextChange
{
    [Export(typeof(IMafPackage))]
    public class MessagePrinterPackage : Package.Package
    {
        public override PackageLoadOption LoadOption => PackageLoadOption.OnContextActivated;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{B765F5CC-554A-4506-8CA0-CC956CC723EE}");

        public MessagePrinterPackage()
        {
            TriggerUiContextViewModel.TestContext.WhenActivated(Initialize);
        }

        public override void Initialize()
        {
            base.Initialize();
            MessageBox.Show("Initialized");
        }
    }
}
