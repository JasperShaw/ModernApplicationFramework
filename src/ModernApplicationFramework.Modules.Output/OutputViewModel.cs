using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Editor.OutputClassifier;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;

namespace ModernApplicationFramework.Modules.Output
{
    [Export(typeof(IOutputPane))]
    [Guid("BF574EA3-975C-4C7B-8099-BCF027FE3632")]
    internal sealed class OutputViewModel : Tool, IOutputPane, IOutputWindowDataSource
    {
        private FrameworkElement _activePane;
        private FrameworkElement _pendingFocusPane;

        public override PaneLocation PreferredLocation => PaneLocation.Bottom;

        public FrameworkElement ActivePane
        {
            get => _activePane;
            set
            {
                if (Equals(value, _activePane)) return;
                _activePane = value;
                NotifyOfPropertyChange();
            }
        }

        public OutputViewModel()
        {
            DisplayName = "Output";

            var output = IoC.Get<IOutput>();
            if (output is IOutputPrivate privateOutput)
                ActivePane = privateOutput.Content as FrameworkElement;
        }
    }
}