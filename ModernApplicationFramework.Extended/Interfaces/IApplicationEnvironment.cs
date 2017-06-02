namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface IApplicationEnvironment
    {
        string LocalAppDataPath { get; }

        void Setup();

        void PrepareClose();
    }
}