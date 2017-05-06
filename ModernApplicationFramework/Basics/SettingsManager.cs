using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Basics
{
    [Export(typeof(SettingsManager))]
    public class SettingsManager
    {
    }


    //[Export(typeof(ISettingContainer))]
    //public class EnvironmentGeneralOptions : ISettingContainer
    //{
        
    //}



    public interface ISettingContainer
    {
        
    }
}
