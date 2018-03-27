using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Modules.Output
{
    [Export(typeof(IOutput))]
    public sealed class OutputViewModel : Tool, IOutput
    {
        private readonly StringBuilder _stringBuilder;
        private readonly OutputWriter _writer;
        private IOutputView _view;

        public IOutputView View
        {
            get => _view;
            set
            {
                if (Equals(value, _view)) return;
                _view = value;
                NotifyOfPropertyChange();
            }
        }

        public ICommand ClearCommand => new Command(Clear);

        public OutputViewModel()
        {
            DisplayName = "Output";
            _stringBuilder = new StringBuilder();
            _writer = new OutputWriter(this);
        }

        public override PaneLocation PreferredLocation => PaneLocation.Bottom;

        public TextWriter Writer => _writer;

        public void Clear()
        {
            if (_view != null)
                Execute.OnUIThread(() => _view.Clear());
            _stringBuilder.Clear();
        }

        public void AppendLine(string text)
        {
            Append(text + Environment.NewLine);
        }

        public void Append(string text)
        {
            _stringBuilder.Append(text);
            OnTextChanged();
        }

        protected override void OnViewLoaded(object view)
        {
            _view = (IOutputView) view;
            _view.SetText(_stringBuilder.ToString());
            _view.ScrollToEnd();
        }

        private void OnTextChanged()
        {
            if (_view != null)
                Execute.OnUIThread(() => _view.SetText(_stringBuilder.ToString()));
        }

        public override ImageSource IconSource => null;
    }
}