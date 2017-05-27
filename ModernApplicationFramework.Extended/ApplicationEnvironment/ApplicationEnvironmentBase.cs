using System;
using System.ComponentModel.Composition;
using System.IO;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.ApplicationEnvironment
{
    [Export(typeof(IApplicationEnvironment))]
    public class ApplicationEnvironmentBase : IApplicationEnvironment
    {
        private readonly IExtendedEnvironmentVarirables _environmentVarirables;
        private readonly ISettingsManager _settingsManager;

        /// <summary>
        /// Tells the Apllication to setup and use the settingsManager.
        /// <remarks>The SettingsCommand should be added to the exclude Commands list</remarks>
        /// </summary>
        protected virtual bool UseApplicationSettings => true;

        [ImportingConstructor]
        public ApplicationEnvironmentBase(IExtendedEnvironmentVarirables environmentVarirables, ISettingsManager settingsManager)
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
                if (UseApplicationSettings  && !Directory.Exists(realSettingsLoc))
                    Directory.CreateDirectory(realSettingsLoc);
            }
            catch (Exception e) when (e is UnauthorizedAccessException)
            {
            }
            if (UseApplicationSettings)
                _settingsManager.Initialize();
        }

        public virtual void PrepareClose()
        {
            if (UseApplicationSettings)
                _settingsManager.Close();
        }
    }
}
