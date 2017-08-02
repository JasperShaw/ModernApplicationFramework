using System;
using System.Windows.Input;
using ModernApplicationFramework.Input.Base;
using Action = System.Action;

namespace ModernApplicationFramework.Input.Command
{
    public class UICommand : AbstractCommandWrapper
    {
        public UICommand(Action executeAction, Func<bool> cantExectueFunc) : base(executeAction, cantExectueFunc)
        {
        }
        
        public UICommand(ICommand wrappedCommand) : base(wrappedCommand)
        {
        }
    }
}
