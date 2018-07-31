using System.ComponentModel.Composition;
using ModernApplicationFramework.TextEditor.Text.Formatting;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ILineTransformSourceProvider))]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class InterLineAdornmentManagerFactory : ILineTransformSourceProvider
    {
        [Export]
        [Name("Inter Line Adornment")]
        [Order(After = "SelectionAndProvisionHighlight", Before = "Squiggle")]
        internal AdornmentLayerDefinition AdornmentLayer;

        [Import]
        internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }

        public ILineTransformSource Create(ITextView textView)
        {
            return InterLineAdornmentManager.Create(textView, this);
        }
    }
}