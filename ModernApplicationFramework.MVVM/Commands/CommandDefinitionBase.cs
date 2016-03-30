using System;

namespace ModernApplicationFramework.MVVM.Commands
{
    public abstract class CommandDefinitionBase
    {
        public abstract Uri IconSource { get; }
        public abstract bool IsList { get; }
        public abstract string Name { get; }
        public abstract string Text { get; }
        public abstract string ToolTip { get; }
    }
}