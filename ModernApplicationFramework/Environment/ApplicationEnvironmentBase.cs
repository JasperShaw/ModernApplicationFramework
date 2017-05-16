using System;
using System.ComponentModel.Composition;
using System.IO;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Interfaces;
using Xamarin.Forms.Xaml;

namespace ModernApplicationFramework.Environment
{
    [Export(typeof(IApplicationEnvironment))]
    public class ApplicationEnvironmentBase : IApplicationEnvironment
    {
        private readonly IEnvironmentVarirables _environmentVarirables;

        [ImportingConstructor]
        public ApplicationEnvironmentBase(IEnvironmentVarirables environmentVarirables)
        {
            _environmentVarirables = environmentVarirables;
        }

        public virtual void Setup()
        {
            _environmentVarirables.GetEnvironmentVariable(_environmentVarirables.ApplicationUserDirectoryKey,
                out string location);
            _environmentVarirables.GetEnvironmentVariable(_environmentVarirables.SettingsDirectoryKey,
                out var settingsLoc);

            var realLoc = System.Environment.ExpandEnvironmentVariables(location);
            var realSettingsLoc = _environmentVarirables.ExpandEnvironmentVariables(settingsLoc);

            try
            {
                if (!Directory.Exists(realLoc))
                    Directory.CreateDirectory(realLoc);
                if (!Directory.Exists(realSettingsLoc))
                    Directory.CreateDirectory(realSettingsLoc);
            }
            catch (Exception e) when (e is UnauthorizedAccessException)
            {
            }

            IoC.Get<EnvironmentGeneralOptions>().Load();
        }
    }

    public interface IApplicationEnvironment
    {
        void Setup();
    }
}
