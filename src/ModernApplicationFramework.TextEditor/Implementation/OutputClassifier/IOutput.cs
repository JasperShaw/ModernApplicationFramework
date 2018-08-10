using System;

namespace ModernApplicationFramework.TextEditor.Implementation.OutputClassifier
{
    public interface IOutput
    {
        event EventHandler ConsoleCleared;

        void OutputString(string text);

        void Clear();

        void Activate();

        void Hide();
    }
}