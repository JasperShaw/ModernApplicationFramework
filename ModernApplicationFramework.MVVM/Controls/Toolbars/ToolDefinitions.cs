﻿using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Controls.Toolbars
{
    public static class ToolDefinitions
    {
        [Export]
        public static ToolbarDefinition Standard = new ToolbarDefinition<StandardToolbar>(0, true, Dock.Top);
    }
}
