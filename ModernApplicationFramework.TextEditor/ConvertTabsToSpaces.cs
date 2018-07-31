using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("Tabs/ConvertTabsToSpaces")]
    public sealed class ConvertTabsToSpaces : EditorOptionDefinition<bool>
    {
        public override bool Default => true;

        public override EditorOptionKey<bool> Key => DefaultOptions.ConvertTabsToSpacesOptionId;
    }
}