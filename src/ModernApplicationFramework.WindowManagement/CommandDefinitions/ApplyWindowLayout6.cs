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
    [Export(typeof(ApplyWindowLayout6))]
    public sealed class ApplyWindowLayout6 : ApplyWindowLayoutBase
    {
        public override int Index => 6;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.D6, ModifierKeys.Control | ModifierKeys.Alt))
        });

        [ImportingConstructor]
        public ApplyWindowLayout6()
        {
            SetCommand();
        }

        public override Guid Id => new Guid("{35AA1060-D400-41BE-8145-EBCCEDC9269C}");
    }
}