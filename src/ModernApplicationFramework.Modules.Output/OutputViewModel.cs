using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.TextEditor;

namespace ModernApplicationFramework.Modules.Output
{
    [Export(typeof(IOutput))]
    public sealed class OutputViewModel : Tool, IOutput, IOutputWindowDataSource
    {
        private readonly OutputWriter _writer;
        private FrameworkElement _activePane;

        public OutputViewModel()
        {
            DisplayName = "Output";
            _writer = new OutputWriter(this);

            var factory = IoC.Get<ITextEditorFactoryService>();

            var view = factory.CreateTextView();
            var host = factory.CreateTextViewHost(view, true);

            ActivePane = host.HostControl;
        }

        public override PaneLocation PreferredLocation => PaneLocation.Bottom;

        public TextWriter Writer => _writer;

        public void Clear()
        {
        }

        public void AppendLine(string text)
        {
        }

        public void Append(string text)
        {
        }

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
    }
}