namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface IApplicationEnvironment
    {
        string LocalAppDataPath { get; }

        string AppDataPath { get; }

        bool UseApplicationSettings { get; }

        void Setup();

        void PrepareClose();
    }
}