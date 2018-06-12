﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.InfoBar;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Package;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.State;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.WindowManagement;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.Extended.Demo.Modules.ToolboxStatePersister
{
    [Export(typeof(IMafPackage))]
    public class StatePersisterPackage : Package.Package, IInfoBarUiEvents
    {
        private readonly IDockingMainWindowViewModel _mainWindow;
        private readonly IToolboxStateSerializer _serializer;
        private readonly ToolboxStateSettings _settings;
        public override PackageLoadOption LoadOption => PackageLoadOption.OnMainWindowLoaded;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{AD27146F-101A-4C39-A0F0-07C809EA53D7}");

        [ImportingConstructor]
        public StatePersisterPackage(IDockingMainWindowViewModel mainWindow, IToolboxStateSerializer serializer, ToolboxStateSettings settings)
        {
            _mainWindow = mainWindow;
            _serializer = serializer;
            _settings = settings;
        }


        public override void Initialize()
        {
            base.Initialize();
            Deserialize();
        }

        protected override void DisposeManagedResources()
        {
            _settings.StoreSettings();
        }

        public void OnActionItemClicked(IInfoBarUiElement infoBarUiElement, IInfoBarActionItem actionItem)
        {
        }

        public void OnClosed(IInfoBarUiElement infoBarUiElement)
        {
        }

        private void Deserialize()
        {
            var layout = _settings.Layout;

            if (layout == null)
                return;

            //if (_serializer.Validate(layout))
            //{
            _serializer.Deserialize(layout);
            //}

            var infoBarTextSpanArray = new[]
            {
                new InfoBarTextSpan("Loaded Toolbox")
            };


            var imageInfo =
                new ImageInfo(
                    "/ModernApplicationFramework.WindowManagement;component/Resources/StatusInformation_16x.png");
            var model = new InfoBarModel(infoBarTextSpanArray, new List<IInfoBarActionItem>(), imageInfo);
            var ui = IoC.Get<IInfoBarUiFactory>().CreateInfoBar(model);
            ui.Advise(this, out _);
            _mainWindow.InfoBarHost.AddInfoBar(ui);

        }
    }
}
