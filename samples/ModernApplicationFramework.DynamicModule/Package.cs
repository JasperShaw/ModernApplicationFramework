using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.DynamicModule
{
    [Export(typeof(IMafPackage))]
    public sealed class Package : Extended.Package.Package
    {
        public override PackageLoadOption LoadOption => PackageLoadOption.Custom;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{FF5ACE0B-DB3F-4BD5-890E-36FE84CBE6B1}");


        public override void Initialize()
        {
            IoC.Get<IDockingMainWindowViewModel>().DockingHost.ShowTool<ITestPrinter>();
        }
    }
}
