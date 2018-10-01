namespace ModernApplicationFramework.Text.Ui.Text
{
    public interface IBraceCompletionContext
    {
        void Start(IBraceCompletionSession session);

        void Finish(IBraceCompletionSession session);

        void OnReturn(IBraceCompletionSession session);

        bool AllowOverType(IBraceCompletionSession session);
    }
}