namespace ModernApplicationFramework.Editor.Implementation
{
    public interface ICommandBindingMetadata
    {
        string[] CommandSet { get; }

        uint[] CommandId { get; }

        string[] CommandArgsType { get; }
    }
}