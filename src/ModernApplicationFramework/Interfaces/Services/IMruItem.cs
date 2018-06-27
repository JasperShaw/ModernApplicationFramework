namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IMruItem : IHasTextProperty
    {
        bool Pinned { get; set; }

        object PersistenceData { get; }

        bool Matches(object data);
    }
}