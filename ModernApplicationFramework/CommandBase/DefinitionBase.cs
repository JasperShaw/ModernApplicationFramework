using System;

namespace ModernApplicationFramework.CommandBase
{
    public abstract class DefinitionBase
    {
        public abstract string Name { get; }
        public abstract string Text { get; }
        public abstract string ToolTip { get; }
        public abstract Uri IconSource { get; }
        public abstract string IconId { get; }
        public abstract bool IsList { get; }
    }
}
