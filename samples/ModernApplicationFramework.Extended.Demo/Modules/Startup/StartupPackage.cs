using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Package;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Inspector;
using ModernApplicationFramework.Modules.Output;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Extended.Demo.Modules.Startup
{
    [Export(typeof(IMafPackage))]
    public class StartupPackage : Package.Package
    {
        private readonly IOutput _output;
        private readonly IInspectorTool _inspectorTool;
        private readonly IStatusBarDataModelService _statusBarService;
        private readonly IToolboxService _toolboxService;
        public override PackageLoadOption LoadOption => PackageLoadOption.OnMainWindowLoaded;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;

        public override Guid Id => new Guid("{0C922534-3BBA-43D1-82DC-EBF3B024F13A}");

        public override IEnumerable<Type> DefaultTools
        {
            get { yield return typeof(IInspectorTool); }
        }

        [ImportingConstructor]
        public StartupPackage(IOutput output, IInspectorTool inspectorTool, IStatusBarDataModelService statusBarService, IToolboxService toolboxService)
        {
            _output = output;
            _inspectorTool = inspectorTool;
            _statusBarService = statusBarService;
            _toolboxService = toolboxService;
        }

        public override void Initialize()
        {
            DockingHostViewModel.ShowFloatingWindowsInTaskbar = true;

            _output.AppendLine("Started up");
            _statusBarService.SetBackgroundColor(AbstractStatusBarService.DefaultColors.Blue);
            _statusBarService.SetText(1, "Test");

            DockingHostViewModel.ActiveLayoutItemChanged += (sender, e) => RefreshInspector();
            RefreshInspector();

            InitializeToolbox();

            base.Initialize();
        }

        private void InitializeToolbox()
        {
            var i = new ToolboxItem("Test", typeof(int), new[] { typeof(ILayoutItem) });
            var j = new ToolboxItem("String", typeof(string), new[] { typeof(object) });
            var c = _toolboxService.GetCategoryById(ToolboxItemCategory.DefaultCategoryId);
            c.Items.Add(i);
            c.Items.Add(j);
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
