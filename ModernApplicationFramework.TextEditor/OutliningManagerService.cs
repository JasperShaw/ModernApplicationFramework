using System;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IOutliningManagerService))]
    internal class OutliningManagerService : IOutliningManagerService
    {
        [Import]
        internal IBufferTagAggregatorFactoryService TagAggregatorFactory { get; set; }

        [Import]
        internal IEditorOptionsFactoryService EditorOptionsFactoryService { get; set; }

        public IOutliningManager GetOutliningManager(ITextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (!textView.Roles.Contains("STRUCTURED"))
                return null;
            return textView.Properties.GetOrCreateSingletonProperty(() =>
            {
                ITagAggregator<IOutliningRegionTag> tagAggregator = TagAggregatorFactory.CreateTagAggregator<IOutliningRegionTag>(textView.TextBuffer);
                OutliningManager manager = new OutliningManager(textView.TextBuffer, tagAggregator, EditorOptionsFactoryService.GlobalOptions);
                textView.Closed += ((_param1, _param2) => manager.Dispose());
                return manager;
            });
        }
    }
}