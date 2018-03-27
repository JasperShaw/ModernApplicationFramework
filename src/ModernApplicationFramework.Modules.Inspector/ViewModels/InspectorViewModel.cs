using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;

namespace ModernApplicationFramework.Modules.Inspector.ViewModels
{
    [Export(typeof(IInspectorTool))]
    public sealed class InspectorViewModel : Tool, IInspectorTool
    {
        private IInspectableObject _selectedObject;
        public event EventHandler SelectedObjectChanged;

        public InspectorViewModel()
        {
            DisplayName = "Inspector";
        }

        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public override double PreferredWidth => 300;

        public IInspectableObject SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                NotifyOfPropertyChange(() => SelectedObject);
                RaiseSelectedObjectChanged();
            }
        }

        private void RaiseSelectedObjectChanged()
        {
            var handler = SelectedObjectChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public override ImageSource IconSource => null;
    }
}