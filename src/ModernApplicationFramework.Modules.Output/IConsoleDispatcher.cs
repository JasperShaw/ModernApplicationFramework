using System;

namespace ModernApplicationFramework.Modules.Output
{
    public interface IConsoleDispatcher
    {
        void Start();

        event EventHandler StartCompleted;

        event EventHandler StartWaitingKey;

        bool IsStartCompleted { get; }

        bool IsExecutingCommand { get; }

        bool IsExecutingReadKey { get; }

        bool IsKeyAvailable { get; }

        void AcceptKeyInput();

        KeyInfo WaitKey();

        void ClearConsole();
    }
}