using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.WindowManagement.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(ApplyWindowLayout5))]
    public sealed class ApplyWindowLayout5 : ApplyWindowLayoutBase
    {
        public override int Index => 5;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.D5, ModifierKeys.Control | ModifierKeys.Alt))
        });


        [ImportingConstructor]
        public ApplyWindowLayout5()
        {
            SetCommand();
        }

        public override Guid Id => new Guid("{93CC5445-1FD1-4E9F-910F-34E96B2B8B32}");
    }
}