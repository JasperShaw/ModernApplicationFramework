﻿using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class CommandSplitButtonDefinition : CommandDefinition
    {
        public override CommandControlTypes ControlType => CommandControlTypes.SplitDropDown;

        public abstract IObservableCollection<object> Items { get; set; }

        public abstract void Execute(int count);
    }
}
