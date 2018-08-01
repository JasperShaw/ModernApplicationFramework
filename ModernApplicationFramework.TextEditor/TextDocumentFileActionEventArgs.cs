using System;

namespace ModernApplicationFramework.TextEditor
{
    public class TextDocumentFileActionEventArgs : EventArgs
    {
        public TextDocumentFileActionEventArgs(string filePath, DateTime time, FileActionTypes fileActionType)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Time = time;
            FileActionType = fileActionType;
        }

        public string FilePath { get; }

        public DateTime Time { get; }

        public FileActionTypes FileActionType { get; }
    }
}