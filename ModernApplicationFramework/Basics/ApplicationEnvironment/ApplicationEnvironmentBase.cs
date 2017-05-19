using System;
using System.ComponentModel.Composition;
using System.IO;
using ModernApplicationFramework.Basics.SettingsManager;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.ApplicationEnvironment
{
    [Export(typeof(IApplicationEnvironment))]
    public class ApplicationEnvironmentBase : IApplicationEnvironment
    {
        private readonly IEnvironmentVarirables _environmentVarirables;
        private readonly ISettingsManager _settingsManager;

        [ImportingConstructor]
        public ApplicationEnvironmentBase(IEnvironmentVarirables environmentVarirables, ISettingsManager settingsManager)
        {
            _environmentVarirables = environmentVarirables;
            _settingsManager = settingsManager;
        }

        public virtual void Setup()
        {
            _environmentVarirables.GetEnvironmentVariable(_environmentVarirables.ApplicationUserDirectoryKey,
                out string location);
            _environmentVarirables.GetEnvironmentVariable(_environmentVarirables.SettingsDirectoryKey,
                out var settingsLoc);

            var realLoc = Environment.ExpandEnvironmentVariables(location);
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
            _settingsManager.Initialize();
        }

        public virtual void PrepareClose()
        {
            _settingsManager.Close();
        }
    }

    public interface IApplicationEnvironment
    {
        void Setup();

        void PrepareClose();
    }
}
