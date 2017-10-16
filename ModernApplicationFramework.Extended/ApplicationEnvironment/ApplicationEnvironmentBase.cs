using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
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

        /// <summary>
        /// Tells the application to setup and use the settingsManager.
        /// <remarks>The SettingsCommand should be added to the exclude Commands list</remarks>
        /// </summary>
        public virtual bool UseApplicationSettings => true;


        public string LocalAppDataPath { get; }

        [ImportingConstructor]
        public ApplicationEnvironmentBase(IExtendedEnvironmentVariables environmentVariables, ISettingsManager settingsManager)
        {
            _environmentVariables = environmentVariables;
            _settingsManager = settingsManager;

            LocalAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
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
            var modules = IoC.GetAll<IModule>().ToList();
            modules.ForEach(x => x.Dispose());
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
            var modules = IoC.GetAll<IModule>().ToList();
            foreach (var module in modules)
            foreach (var globalResourceDictionary in module.GlobalResourceDictionaries)
                Application.Current.Resources.MergedDictionaries.Add(globalResourceDictionary);

            //TODO: Create a global Module-Manager
            foreach (var module in modules)
                module.PreInitialize();
        }
    }
}
