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
        private readonly IExtendedEnvironmentVarirables _environmentVarirables;
        private readonly ISettingsManager _settingsManager;

        /// <summary>
        /// Tells the Apllication to setup and use the settingsManager.
        /// <remarks>The SettingsCommand should be added to the exclude Commands list</remarks>
        /// </summary>
        protected virtual bool UseApplicationSettings => true;


        public string LocalAppDataPath { get; }

        [ImportingConstructor]
        public ApplicationEnvironmentBase(IExtendedEnvironmentVarirables environmentVarirables, ISettingsManager settingsManager)
        {
            _environmentVarirables = environmentVarirables;
            _settingsManager = settingsManager;

            LocalAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                environmentVarirables.ApplicationName, environmentVarirables.ApplicationVersion);
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
                if (!Directory.Exists(LocalAppDataPath))
                    Directory.CreateDirectory(LocalAppDataPath);
            }
            catch (Exception e) when (e is UnauthorizedAccessException)
            {
            }
            if (UseApplicationSettings)
                _settingsManager.Initialize();


            var modules = IoC.GetAll<IModule>().ToList();
            foreach (var module in modules)
            foreach (var globalResourceDictionary in module.GlobalResourceDictionaries)
                Application.Current.Resources.MergedDictionaries.Add(globalResourceDictionary);


            foreach (var module in modules)
                module.PreInitialize();
        }

        public virtual void PrepareClose()
        {
            var modules = IoC.GetAll<IModule>().ToList();
            modules.ForEach(x => x.Dispose());



            if (UseApplicationSettings)
                _settingsManager.Close();
        }
    }
}
