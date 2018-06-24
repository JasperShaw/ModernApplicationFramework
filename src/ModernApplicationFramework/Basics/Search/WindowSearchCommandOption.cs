using System;
using ModernApplicationFramework.Interfaces.Search;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Search
{
    public class WindowSearchCommandOption : WindowSearchOption, IWindowSearchCommandOption
    {
        private readonly Action _commandAction;

        public WindowSearchCommandOption(string displayText, string tooltip, Action commandAction)
            : base(displayText, tooltip)
        {
            Validate.IsNotNull(commandAction, nameof(commandAction));
            _commandAction = commandAction;
        }

        public virtual void Invoke()
        {
            _commandAction();
        }
    }
}