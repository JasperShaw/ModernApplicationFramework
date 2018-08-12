using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Editor.OutputClassifier;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Package;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Inspector;
using ModernApplicationFramework.Modules.Toolbox;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Extended.Demo.Modules.Startup
{
    [Export(typeof(IMafPackage))]
    [PackageAutoLoad(UiContextGuids.ShellInitializedContextGuid)]
    public class StartupPackage : Package.Package
    {
        private readonly IOutput _outputPane;
        private readonly IInspectorTool _inspectorTool;
        private readonly IStatusBarDataModelService _statusBarService;
        private readonly IToolboxService _toolboxService;
        public override PackageLoadOption LoadOption => PackageLoadOption.OnContextActivated;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;

        public override Guid Id => new Guid("{0C922534-3BBA-43D1-82DC-EBF3B024F13A}");

        public override IEnumerable<Type> DefaultTools
        {
            get { yield return typeof(IInspectorTool); }
        }

        [ImportingConstructor]
        public StartupPackage(IOutput outputPane, IInspectorTool inspectorTool, IStatusBarDataModelService statusBarService, IToolboxService toolboxService)
        {
            _outputPane = outputPane;
            _inspectorTool = inspectorTool;
            _statusBarService = statusBarService;
            _toolboxService = toolboxService;
        }

        protected override void Initialize()
        {
            DockingHostViewModel.ShowFloatingWindowsInTaskbar = true;

            _outputPane.OutputString("Started up");
            _statusBarService.SetBackgroundColor(AbstractStatusBarService.DefaultColors.Blue);
            _statusBarService.SetText(1, "Test");

            DockingHostViewModel.ActiveLayoutItemChanged += (sender, e) => RefreshInspector();
            RefreshInspector();

            InitializeToolbox();

            base.Initialize();
        }

        private void InitializeToolbox()
        {
            var data = new ToolboxItemDefinition("Test", new ToolboxItemData(ToolboxItemDataFormats.Type, typeof(int)),
                new[] {typeof(ILayoutItem)}, default, false);
            var i = new ToolboxItem(data);

            var c = _toolboxService.GetCategoryById(Guids.DefaultCategoryId);
            c.Items.Add(i);
        }

        private void RefreshInspector()
        {
            if (DockingHostViewModel.ActiveItem != null)
                _inspectorTool.SelectedObject = new InspectableObjectBuilder()
                    .WithObjectProperties(DockingHostViewModel.ActiveItem, pd => pd.ComponentType == DockingHostViewModel.ActiveItem.GetType())
                    .ToInspectableObject();
            else
                _inspectorTool.SelectedObject = null;
        }
    }
}
