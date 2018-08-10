﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/ProduceScreenReaderFriendlyText")]
    public sealed class ProduceScreenReaderFriendlyText : ViewOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultTextViewOptions.ProduceScreenReaderFriendlyTextId;
    }
}
