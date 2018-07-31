using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("Tabs/TabSize")]
    public sealed class TabSize : EditorOptionDefinition<int>
    {
        public override int Default => 4;

        public override EditorOptionKey<int> Key => DefaultOptions.TabSizeOptionId;

        public override bool IsValid(ref int proposedValue)
        {
            return proposedValue > 0;
        }
    }
}