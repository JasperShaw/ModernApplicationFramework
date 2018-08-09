using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.TextEditor.Implementation.OutputClassifier;

namespace ModernApplicationFramework.Modules.Output
{
    [Export(typeof(IOutputPane))]
    [Guid("BF574EA3-975C-4C7B-8099-BCF027FE3632")]
    internal sealed class OutputViewModel : Tool, IOutputPane, IOutputWindowDataSource
    {
        private FrameworkElement _activePane;

        public OutputViewModel()
        {
            DisplayName = "Output";

            //var factory = IoC.Get<ITextEditorFactoryService>();
            //var view = factory.CreateTextView();
            //var host = factory.CreateTextViewHost(view, true);

            //var factory = IoC.Get<IEditorAdaptersFactoryService>();
            //var bufferModel = factory.CreateTextBufferAdapter();

            //var data = "Hallo Welt";
            //bufferModel.InitializeContent(data, data.Length);

            //var tva = factory.CreateTextViewAdapter();
            //tva.SetBuffer(bufferModel as IMafTextLines);


            //var host = factory.GetTextViewHost(tva);

            //ActivePane = host.HostControl;

            var output = IoC.Get<IOutput>();
            if (output is IOutputPrivate privateOutput)
                ActivePane = privateOutput.Content as FrameworkElement;
        }

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
    }
}