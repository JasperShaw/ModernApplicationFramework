using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace MordernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ApplyWindowLayout3))]
    public sealed class ApplyWindowLayout3 : ApplyWindowLayoutBase
    {
        public override int Index => 3;
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }


        [ImportingConstructor]
        public ApplyWindowLayout3()
        {
            DefaultKeyGesture = new MultiKeyGesture(Key.D3, ModifierKeys.Control | ModifierKeys.Alt);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }
}