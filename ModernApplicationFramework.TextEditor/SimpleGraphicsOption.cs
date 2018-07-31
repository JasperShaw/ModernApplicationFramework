﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("Graphics/Simple/Enable")]
    public sealed class SimpleGraphicsOption : ViewOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultViewOptions.EnableSimpleGraphicsId;
    }
}