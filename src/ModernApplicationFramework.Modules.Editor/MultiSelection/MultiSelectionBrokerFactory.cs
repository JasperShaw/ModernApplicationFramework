using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    [Export(typeof(IMultiSelectionBrokerFactory))]
    //[Export(typeof(IFeatureController))]
    internal class MultiSelectionBrokerFactory : IMultiSelectionBrokerFactory//, IFeatureController
    {
        [Import]
        internal ISmartIndentationService SmartIndentationService { get; set; }

        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService { get; set; }

        [Import]
        internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        //[Import(AllowDefault = true)]
        //internal ILoggingServiceInternal LoggingService { get; set; }

        //[Import]
        //internal IFeatureServiceFactory FeatureServiceFactory { get; set; }

        [Import]
        internal IEditorOptionsFactoryService EditorOptionsFactoryService { get; set; }

        [Import]
        internal IGuardedOperations GuardedOperations { get; set; }

        public IMultiSelectionBroker CreateBroker(ITextView textView)
        {
            return new MultiSelectionBroker(textView, this);
        }
    }
}
