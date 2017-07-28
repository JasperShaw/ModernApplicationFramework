﻿using System;
using System.Windows.Input;
using Action = System.Action;

namespace ModernApplicationFramework.CommandBase
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