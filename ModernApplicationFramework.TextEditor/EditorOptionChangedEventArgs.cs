using System;

namespace ModernApplicationFramework.TextEditor
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