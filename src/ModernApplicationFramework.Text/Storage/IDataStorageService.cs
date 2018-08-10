namespace ModernApplicationFramework.Text.Storage
{
    public interface IDataStorageService
    {
        IDataStorage GetDataStorage(string storageKey);
    }
}