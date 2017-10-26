﻿using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using MordernApplicationFramework.WindowManagement.LayoutManagement;

namespace MordernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ApplyWindowLayout1))]
    public sealed class ApplyWindowLayout1 : ApplyWindowLayoutBase
    {
        public override int Index => 1;
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        protected override ILayoutManager LayoutManager { get; }

        [ImportingConstructor]
        public ApplyWindowLayout1(ILayoutManager layoutManager)
        {
            LayoutManager = layoutManager;
            DefaultKeyGesture = new MultiKeyGesture(Key.D1, ModifierKeys.Control | ModifierKeys.Alt);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        } 
    }
}