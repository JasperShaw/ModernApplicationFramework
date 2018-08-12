using System;

namespace ModernApplicationFramework.Editor.OutputClassifier
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