using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("LongBufferLineThreshold")]
    public sealed class LongBufferLineThreshold : EditorOptionDefinition<int>
    {
        public override int Default => 32768;

        public override EditorOptionKey<int> Key => DefaultOptions.LongBufferLineThresholdId;
    }
}