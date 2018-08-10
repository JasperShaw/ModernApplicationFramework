using System;

namespace ModernApplicationFramework.Text.Logic.Editor
{
    public class EditorOptionChangedEventArgs : EventArgs
    {
        public string OptionId { get; }

        public EditorOptionChangedEventArgs(string optionId)
        {
            OptionId = optionId;
        }
    }
}