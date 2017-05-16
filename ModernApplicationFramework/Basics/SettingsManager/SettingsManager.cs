namespace ModernApplicationFramework.Basics.SettingsManager
{
    public class SettingsManager : ISettingsManager
    {
        public SettingsManager()
        {
            
        }
    }




    public interface ISettingsManager
    {
        //Task SetValueAsync(string name, object value, bool isMachineLocal);

        //T GetValueOrDefault<T>(string name, T defaultValue);

        //GetValueResult TryGetValue<T>(string name, out T value);

        //ISettingsList GetOrCreateList(string name, bool isMachineLocal);

        //string[] NamesStartingWith(string prefix);

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

