using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.WindowManagement.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(ApplyWindowLayout1))]
    public sealed class ApplyWindowLayout1 : ApplyWindowLayoutBase
    {
        public override int Index => 1;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.D1, ModifierKeys.Control | ModifierKeys.Alt))
        });


        [ImportingConstructor]
        public ApplyWindowLayout1()
        {
            SetCommand();
        }

        public override Guid Id => new Guid("{C17FB832-108A-4863-9CD0-F3596BAC1CF3}");
    }
}