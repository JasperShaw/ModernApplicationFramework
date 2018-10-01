namespace ModernApplicationFramework.Text.Ui.Text
{
    public interface IBraceCompletionManager
    {
        bool Enabled { get; }

        bool HasActiveSessions { get; }

        string OpeningBraces { get; }

        string ClosingBraces { get; }

        void PreTypeChar(char character, out bool handledCommand);

        void PostTypeChar(char character);

        void PreTab(out bool handledCommand);

        void PostTab();

        void PreBackspace(out bool handledCommand);

        void PostBackspace();

        void PreDelete(out bool handledCommand);

        void PostDelete();

        void PreReturn(out bool handledCommand);

        void PostReturn();
    }
}
