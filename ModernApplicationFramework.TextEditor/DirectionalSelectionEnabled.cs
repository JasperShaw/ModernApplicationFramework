﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("DirectionalSelectionEnabled")]
    public sealed class DirectionalSelectionEnabled : EditorOptionDefinition<bool>
    {
        public static readonly EditorOptionKey<bool> DirectionalSelectionEnabledId = new EditorOptionKey<bool>(nameof(DirectionalSelectionEnabled));
        public const string DirectionalSelectionEnabledName = "DirectionalSelectionEnabled";

        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DirectionalSelectionEnabledId;
    }
}