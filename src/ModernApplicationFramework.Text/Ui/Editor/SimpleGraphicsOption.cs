﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("Graphics/Simple/Enable")]
    public sealed class SimpleGraphicsOption : ViewOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultViewOptions.EnableSimpleGraphicsId;
    }
}