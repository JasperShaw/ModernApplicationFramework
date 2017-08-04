using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Core.ModuleBase;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Modules.InspectorTool;
using ModernApplicationFramework.Extended.Modules.OutputTool;

namespace ModernApplicationFramework.Extended.TestWindow.Startup
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {

        private readonly IOutput _output;
        private readonly IInspectorTool _inspectorTool;

        public override IEnumerable<Type> DefaultTools
        {
            get { yield return typeof(IInspectorTool); }
        }

        [ImportingConstructor]
        public Module(IOutput output, IInspectorTool inspectorTool)
        {
            _output = output;
            _inspectorTool = inspectorTool;
            
        }

        public override void Initialize()
        {
            DockingHostViewModel.ShowFloatingWindowsInTaskbar = true;

            _output.AppendLine("Started up");

            DockingHostViewModel.ActiveDocumentChanged += (sender, e) => RefreshInspector();
            RefreshInspector();

            DockingHostViewModel.ShowTool(_output);
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
