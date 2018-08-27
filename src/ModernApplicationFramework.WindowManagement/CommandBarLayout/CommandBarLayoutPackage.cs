using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.InfoBar;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Extended;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Package;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.CommandBarLayout
{
    [Export(typeof(IMafPackage))]
    [PackageAutoLoad(UiContextGuids.ShellInitializedContextGuid)]
    public class CommandBarLayoutPackage : Package, IInfoBarUiEvents
    {
        private readonly IDockingMainWindowViewModel _mainWindow;
        private readonly ICommandBarSerializer _serializer;
        private readonly CommandBarLayoutSettings _settings;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{016D9005-A120-4E35-8BCE-33CF48250C20}");
        public override PackageLoadOption LoadOption => PackageLoadOption.OnContextActivated;

        [ImportingConstructor]
        public CommandBarLayoutPackage(IDockingMainWindowViewModel mainWindow, ICommandBarSerializer serializer,
            CommandBarLayoutSettings settings)
        {
            _mainWindow = mainWindow;
            _serializer = serializer;
            _settings = settings;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Deserialize();
        }

        public void OnActionItemClicked(IInfoBarUiElement infoBarUiElement, IInfoBarActionItem actionItem)
        {
        }

        public void OnClosed(IInfoBarUiElement infoBarUiElement)
        {
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _settings.StoreSettings();
        }

        private void Deserialize()
        {
            //Disable because startup gets very slow with attached debugger
            //if (Debugger.IsAttached)
            //    return;
            var settings = IoC.Get<CommandBarLayoutSettings>();
            var layout = settings.Layout;
            if (layout == null)
                return;
            if (_serializer.Validate(layout))
            {
                _serializer.Deserialize(layout);
            }
            else
            {
                var infoBarTextSpanArray = new[]
                {
                    new InfoBarTextSpan(WindowManagement_Resources.ErrorLoadCommandBarLayout)
                };

                var model = new InfoBarModel(infoBarTextSpanArray, new List<IInfoBarActionItem>(), ImageCatalog.Monikers.StatusInfo);
                var ui = IoC.Get<IInfoBarUiFactory>().CreateInfoBar(model);
                ui.Advise(this, out _);
                _mainWindow.InfoBarHost.AddInfoBar(ui);
            }
        }
    }
}