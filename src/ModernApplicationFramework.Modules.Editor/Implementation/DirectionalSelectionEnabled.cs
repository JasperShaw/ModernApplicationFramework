using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("DirectionalSelectionEnabled")]
    public sealed class DirectionalSelectionEnabled : EditorOptionDefinition<bool>
    {
        public const string DirectionalSelectionEnabledName = "DirectionalSelectionEnabled";

        public static readonly EditorOptionKey<bool> DirectionalSelectionEnabledId =
            new EditorOptionKey<bool>(nameof(DirectionalSelectionEnabled));

        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DirectionalSelectionEnabledId;
    }
}