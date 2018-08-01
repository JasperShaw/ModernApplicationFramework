namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal interface ICommandTargetInner
    {
        int InnerExec();

        int InnerQueryStatus();
    }
}