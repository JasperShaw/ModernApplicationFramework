using System;
using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.Extended.Demo.Modules.UIContextChange
{
    [Export(typeof(IMafPackage))]
    [PackageAutoLoad("CA2D40CF-F606-4FE6-ABEB-5B3E07839C55")]
    [PackageAutoLoad("4C800FF2-6D1D-45DE-80A2-9ADE26037208")]
    public class MessagePrinterPackage2 : Package.Package
    {
        public override PackageLoadOption LoadOption => PackageLoadOption.OnContextActivated;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{B765F5CC-554A-4506-8CA0-CC956CC723EE}");

        protected override void Initialize()
        {
            base.Initialize();
            Print();
        }

        public void Print()
        {
            MessageBox.Show("Initialized 2");
        }
    }
}
