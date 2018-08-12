using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Outlining;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Modules.Editor.Outlining
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
                var tagAggregator = TagAggregatorFactory.CreateTagAggregator<IOutliningRegionTag>(textView.TextBuffer);
                var manager = new OutliningManager(textView.TextBuffer, tagAggregator, EditorOptionsFactoryService.GlobalOptions);
                textView.Closed += (_param1, _param2) => manager.Dispose();
                return manager;
            });
        }
    }
}