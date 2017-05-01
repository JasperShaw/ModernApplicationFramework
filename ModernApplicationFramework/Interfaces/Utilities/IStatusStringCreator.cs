namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IStatusStringCreator
    {
        string StatusTextTemplate { get; set; }
        string PluralSuffix { get; set; }

        string CreateMessage(object value);

        string CreateDefaultMessage();
    }
}