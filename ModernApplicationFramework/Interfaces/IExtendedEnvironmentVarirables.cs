namespace ModernApplicationFramework.Interfaces
{
    public interface IExtendedEnvironmentVarirables : IEnvironmentVarirables
    {
        string SettingsFilePath { get; set; }

        string SettingsDirectoryKey { get; }
    }
}