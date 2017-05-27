namespace ModernApplicationFramework.Interfaces
{
    public interface IEnvironmentVarirables
    {
        string ApplicationName { get; }

        string ApplicationVersion { get; }

        string ApplicationUserDirectoryKey { get; }

        void Setup();

        bool GetEnvironmentVariable(string key, out string value);

        string ExpandEnvironmentVariables(string name);

        string GetOrCreateRegistryVariable(string key, string path, string defaultValue);

        void SetRegistryVariable(string key, string value, string path);
    }
}