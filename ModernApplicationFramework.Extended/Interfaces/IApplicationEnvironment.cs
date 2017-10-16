namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface IApplicationEnvironment
    {
        string LocalAppDataPath { get; }

         bool UseApplicationSettings { get; }

        void Setup();

        void PrepareClose();
    }
}