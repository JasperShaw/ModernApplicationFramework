using System;
using System.ComponentModel.Composition;
using System.IO;
using ModernApplicationFramework.Extended.Core.Package;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.ApplicationEnvironment
{
    [Export(typeof(IApplicationEnvironment))]
    public class ApplicationEnvironmentBase : IApplicationEnvironment
    {
        private readonly IExtendedEnvironmentVariables _environmentVariables;
        private readonly ISettingsManager _settingsManager;
        private IPackageManager _packageManager;


        /// <summary>
        /// Tells the application to setup and use the settingsManager.
        /// <remarks>The SettingsCommand should be added to the exclude Commands list</remarks>
        /// </summary>
        public virtual bool UseApplicationSettings => true;

        public string LocalAppDataPath { get; }

        public string AppDataPath { get; }

        [ImportingConstructor]
        public ApplicationEnvironmentBase(IExtendedEnvironmentVariables environmentVariables, ISettingsManager settingsManager)
        {
            _environmentVariables = environmentVariables;
            _settingsManager = settingsManager;

            LocalAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                environmentVariables.ApplicationName, environmentVariables.ApplicationVersion);

            AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                environmentVariables.ApplicationName, environmentVariables.ApplicationVersion);
        }

        public virtual void Setup()
        {
            SetupApplicationDirectories();
            SetupSettings();
            SetupModules();
        }
        
        public virtual void PrepareClose()
        {
            _packageManager.ClosePackages(PackageCloseOption.PreviewApplicationClosed);
            _packageManager.ShutDown();
            if (UseApplicationSettings)
                _settingsManager.Close();
        }

        protected virtual void SetupSettings()
        {
            _environmentVariables.GetEnvironmentVariable(_environmentVariables.SettingsDirectoryKey,
                out var settingsLoc);
            var realSettingsLoc = _environmentVariables.ExpandEnvironmentVariables(settingsLoc);
            try
            {
                if (UseApplicationSettings && !Directory.Exists(realSettingsLoc))
                    Directory.CreateDirectory(realSettingsLoc);
            }
            catch (Exception e) when (e is UnauthorizedAccessException)
            {
            }
            if (UseApplicationSettings)
                _settingsManager.Initialize();
        }

        protected virtual void SetupApplicationDirectories()
        {
            _environmentVariables.GetEnvironmentVariable(_environmentVariables.ApplicationUserDirectoryKey,
                out string location);
            var realLoc = Environment.ExpandEnvironmentVariables(location);
            try
            {
                if (!Directory.Exists(realLoc))
                    Directory.CreateDirectory(realLoc);
                if (!Directory.Exists(LocalAppDataPath))
                    Directory.CreateDirectory(LocalAppDataPath);
            }
            catch (Exception e) when (e is UnauthorizedAccessException)
            {
            }
        }

        protected virtual void SetupModules()
        {
            _packageManager = new PackageManager();
            _packageManager.Initialize();
            _packageManager.LoadPackages(PackageLoadOption.OnApplicationStart);
        }
    }
}
