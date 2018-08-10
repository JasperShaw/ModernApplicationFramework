using System;

namespace ModernApplicationFramework.Text.Logic.Editor
{
    public class EditorOptionChangedEventArgs : EventArgs
    {
        public EditorOptionChangedEventArgs(string optionId)
        {
            OptionId = optionId;
        }

        public string OptionId { get; }
    }
}