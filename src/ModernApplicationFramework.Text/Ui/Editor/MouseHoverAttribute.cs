using System;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class MouseHoverAttribute : Attribute
    {
        public MouseHoverAttribute(int delay)
        {
            Delay = delay;
        }

        public int Delay { get; }
    }
}