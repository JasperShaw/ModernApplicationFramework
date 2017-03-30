﻿using System;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public class CommandListDefinition : DefinitionBase
    {
        public override string IconId => null;
        public override bool IsList => true;
        public override CommandControlTypes ControlType => CommandControlTypes.Button;
        public override Uri IconSource => null;
        public override string Name => string.Empty;
        public override string Text => Name;
        public override string ToolTip => string.Empty;
    }
}
