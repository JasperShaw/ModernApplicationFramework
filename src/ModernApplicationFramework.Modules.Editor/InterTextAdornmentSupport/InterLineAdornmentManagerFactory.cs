using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.InterTextAdornmentSupport
{
    [Export(typeof(ILineTransformSourceProvider))]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class InterLineAdornmentManagerFactory : ILineTransformSourceProvider
    {
        [Export] [Name("Inter Line Adornment")] [Order(After = "SelectionAndProvisionHighlight", Before = "Squiggle")]
        internal AdornmentLayerDefinition AdornmentLayer;

        [Import] internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }

        public ILineTransformSource Create(ITextView textView)
        {
            return InterLineAdornmentManager.Create(textView, this);
        }
    }
}