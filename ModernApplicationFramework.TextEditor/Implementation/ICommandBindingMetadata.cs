namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface ICommandBindingMetadata
    {
        string[] CommandSet { get; }

        uint[] CommandId { get; }

        string[] CommandArgsType { get; }
    }
}