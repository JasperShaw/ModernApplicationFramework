﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("ImeCompositionWindowTopOffset")]
    public sealed class ImeCompositionWindowTopOffset : EditorOptionDefinition<double>
    {
        public const string OptionName = "ImeCompositionWindowTopOffset";

        public static readonly EditorOptionKey<double> KeyId =
            new EditorOptionKey<double>(nameof(ImeCompositionWindowTopOffset));

        public override double Default => double.NaN;

        public override EditorOptionKey<double> Key => KeyId;
    }
}