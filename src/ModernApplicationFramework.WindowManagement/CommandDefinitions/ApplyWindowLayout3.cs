using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.WindowManagement.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(ApplyWindowLayout3))]
    public sealed class ApplyWindowLayout3 : ApplyWindowLayoutBase
    {
        public override int Index => 3;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.D3, ModifierKeys.Control | ModifierKeys.Alt))
        });


        [ImportingConstructor]
        public ApplyWindowLayout3()
        {
            SetCommand();
        }

        public override Guid Id => new Guid("{47C25585-AE08-496F-BA7E-F9FC07F7F0E6}");
    }
}