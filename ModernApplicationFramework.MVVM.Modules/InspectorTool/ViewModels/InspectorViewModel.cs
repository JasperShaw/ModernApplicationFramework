using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.MVVM.Controls;
using ModernApplicationFramework.MVVM.Core;

namespace ModernApplicationFramework.MVVM.Modules.InspectorTool.ViewModels
{
    [Export(typeof(IInspectorTool))]
    public class InspectorViewModel : Tool, IInspectorTool
    {
        public event EventHandler SelectedObjectChanged;

        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public override double PreferredWidth => 300;

        private IInspectableObject _selectedObject;

        public IInspectableObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                _selectedObject = value;
                NotifyOfPropertyChange(() => SelectedObject);
                RaiseSelectedObjectChanged();
            }
        }

        public InspectorViewModel()
        {
            DisplayName = "Inspector";
        }

        private void RaiseSelectedObjectChanged()
        {
            EventHandler handler = SelectedObjectChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
