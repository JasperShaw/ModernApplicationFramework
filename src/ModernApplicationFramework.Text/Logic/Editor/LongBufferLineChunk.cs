using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Logic.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("LongBufferLineChunkLength")]
    public sealed class LongBufferLineChunk : EditorOptionDefinition<int>
    {
        public override int Default => 4096;

        public override EditorOptionKey<int> Key => DefaultOptions.LongBufferLineChunkLengthId;
    }
}