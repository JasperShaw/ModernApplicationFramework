using System;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Basics
{
    public abstract class SettingsManager
    {
        public abstract EnclosingScopes GetCollectionScopes(string collectionPath);

        public abstract EnclosingScopes GetPropertyScopes(string collectionPath, string propertyName);

        public abstract SettingsStore GetReadOnlySettingsStore(SettingsScope scope);

        public abstract WritableSettingsStore GetWritableSettingsStore(SettingsScope scope);

        public abstract string GetApplicationDataFolder(ApplicationDataFolder folder);
    }

    public enum ApplicationDataFolder
    {
        LocalSettings,
        RoamingSettings,
        Configuration,
        Documents,
        UserExtensions,
        ApplicationExtensions,
    }

    [Flags]
    public enum EnclosingScopes
    {
        None = 0,
        Configuration = 1,
        UserSettings = 2,
        Remote = 4,
    }

    public enum SettingsScope
    {
        Configuration = 1,
        UserSettings = 2,
        Remote = 4,
    }

    public abstract class SettingsStore
    {

    }

    public abstract class WritableSettingsStore : SettingsStore
    {

    }




    public interface ISettingsManager
    {
        Task SetValueAsync(string name, object value, bool isMachineLocal);

        T GetValueOrDefault<T>(string name, T defaultValue);

        GetValueResult TryGetValue<T>(string name, out T value);

        //ISettingsList GetOrCreateList(string name, bool isMachineLocal);

        string[] NamesStartingWith(string prefix);

        //ISettingsSubset GetSubset(string namePattern);
    }



    public enum GetValueResult
    {
        Success,
        Missing,
        Corrupt,
        IncompatibleType,
        ObsoleteFormat,
        UnknownError,
    }
}

