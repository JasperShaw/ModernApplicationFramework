using System;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Packages;
using ModernApplicationFramework.EditorBase.Settings.MruFiles;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.EditorBase.Packages.MruFiles
{
    [Export(typeof(IMafPackage))]
    [Export(typeof(IMruFilePackage))]
    public class MruFilePackage : Package, IMruFilePackage
    {
        private readonly EnvironmentGeneralOptions _options;
        private readonly StoredMruFiles _store;
        public override PackageLoadOption LoadOption => PackageLoadOption.OnMainWindowLoaded;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{054BEBB2-8128-48C5-A656-A93B183DE420}");

        public FileSystemMruManager Manager { get; }

        [ImportingConstructor]
        public MruFilePackage(EnvironmentGeneralOptions options, StoredMruFiles store)
        {
            _options = options;
            _store = store;
            options.PropertyChanged += Options_PropertyChanged;
            Manager = new FileSystemMruManager(options.MRUListItems);
        }

        private void Options_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EnvironmentGeneralOptions.MRUListItems))
                UpdateMaxItems(_options.MRUListItems);
        }

        protected override void Initialize()
        {
            base.Initialize();
            var items = _store.GetSotredItems();
            foreach (var data in items.Reverse())
                Manager.AddItem(data);

        }

        protected override void DisposeManagedResources()
        {
            _store.StoreItems(Manager.Items);
            base.DisposeManagedResources();
        }

        private void UpdateMaxItems(int maxItems)
        {
            Manager.MaxCount = maxItems;
        }
    }
}
