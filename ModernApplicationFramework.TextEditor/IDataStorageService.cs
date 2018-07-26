namespace ModernApplicationFramework.TextEditor
{
    public interface IDataStorageService
    {
        IDataStorage GetDataStorage(string storageKey);
    }
}