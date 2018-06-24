namespace ModernApplicationFramework.Interfaces.Search
{
    public interface IEnumWindowSearchOptions
    {
        int Next(uint celt, IWindowSearchOption[] rgelt, out uint fetched);

        int Skip(uint celt);

        void Reset();

        void Clone(out IEnumWindowSearchOptions options);
    }
}
