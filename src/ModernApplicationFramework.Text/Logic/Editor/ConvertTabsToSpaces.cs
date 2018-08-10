using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Logic.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("Tabs/ConvertTabsToSpaces")]
    public sealed class ConvertTabsToSpaces : EditorOptionDefinition<bool>
    {
        public override bool Default => true;

        public override EditorOptionKey<bool> Key => DefaultOptions.ConvertTabsToSpacesOptionId;
    }
}