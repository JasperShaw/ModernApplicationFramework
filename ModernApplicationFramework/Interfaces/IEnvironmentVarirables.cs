namespace ModernApplicationFramework.Interfaces
{
    public interface IEnvironmentVarirables
    {
        string ApplicationName { get; }

        string ApplicationVersion { get; }

        string SettingsFilePath { get; set; }

        string SettingsDirectoryKey { get; } 
        string ApplicationUserDirectoryKey { get; }

        void Setup();

        bool GetEnvironmentVariable(string key, out string value);

        string ExpandEnvironmentVariables(string name);
    }
}