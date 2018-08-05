using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.Modules.Output
{
    [Export(typeof(IOutput))]
    [Guid("BF574EA3-975C-4C7B-8099-BCF027FE3632")]
    public sealed class OutputViewModel : Tool, IOutput, IOutputWindowDataSource
    {
        private readonly OutputWriter _writer;
        private FrameworkElement _activePane;

        public OutputViewModel()
        {
            DisplayName = "Output";
            _writer = new OutputWriter(this);



            //TODO: Look at VS: OutputWindowTagger.cs and OutputWindowTaggerProvider.cs



            //var factory = IoC.Get<ITextEditorFactoryService>();
            //var view = factory.CreateTextView();
            //var host = factory.CreateTextViewHost(view, true);

            var factory = IoC.Get<IEditorAdaptersFactoryService>();
            var bufferModel = factory.CreateTextBufferAdapter();

            var data = "Hallo Welt";
            bufferModel.InitializeContent(data, data.Length);

            var tva = factory.CreateTextViewAdapter();
            tva.SetBuffer(bufferModel as IMafTextLines);


            var host = factory.GetTextViewHost(tva);

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