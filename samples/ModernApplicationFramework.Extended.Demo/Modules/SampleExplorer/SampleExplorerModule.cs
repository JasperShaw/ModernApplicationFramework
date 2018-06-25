using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer
{
    [Export(typeof(IMafPackage))]
    public class SampleExplorerModule : Package.Package
    {
        public override PackageLoadOption LoadOption => PackageLoadOption.OnMainWindowLoaded;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;

        public override Guid Id => new Guid("{E3C144F0-E251-4BC5-BC83-42FB8B7BCD41}");

        protected override void Initialize()
        {
            DockingHostViewModel.OpenLayoutItem(IoC.Get<SampleViewModel>());
            base.Initialize();
        }
    }
}
