namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal interface ICommandTarget
    {
        int QueryStatus();

        int Exec();
    }
}