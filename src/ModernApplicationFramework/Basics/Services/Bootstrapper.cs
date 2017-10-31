using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.ApplicationEnvironment;
using ModernApplicationFramework.Core.Localization;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Native.TrinetCoreNtfs;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Basics.Services
{
    /// <inheritdoc />
    /// <summary>
    /// A minimal bootsrapper to launch the application
    /// </summary>
    /// <seealso cref="T:Caliburn.Micro.BootstrapperBase" />
    public abstract class Bootstrapper : BootstrapperBase
    {
        /// <summary>
        /// The container of all instances
        /// </summary>
        private CompositionContainer _container;

        /// <summary>
        /// The priority assemblies
        /// </summary>
        private List<Assembly> _priorityAssemblies;

        /// <summary>
        /// Gets the environment variables for this application.
        /// </summary>
        protected virtual IEnvironmentVariables EnvironmentVariables => new FallbackEnvironmentVariables();

        protected Bootstrapper(bool useApplication = true) : base(useApplication)
        {
        }

	    public IEnumerable<Assembly> PriorityAssemblies => _priorityAssemblies;

        protected new void Initialize()
        {
            PreInitialize();
            base.Initialize();
            PostInitialize();
        }

        /// <summary>
        /// Performs actions before the bootstrapper gets initialized.
        /// </summary>
        protected virtual void PreInitialize()
        {
            var exassemlby = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(exassemlby);
            // This makes sure we can share any applications through the Internet
            ClearUrlZonesInDirectory(directory);
        }

        /// <summary>
        /// Performs actions after the bootstrapper gets initialized.
        /// </summary>
        protected virtual void PostInitialize()
        {
            SetLanguage();
        }

        /// <summary>
        /// Sets the language of the application.
        /// </summary>
        private void SetLanguage()
        {
	        var lm = _container.GetExportedValue<ILanguageManager>() as LanguageManager;
			lm?.SetLanguage();
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }

        /// <summary>
        /// Exports object instances
        /// </summary>
        /// <param name="batch">The batch.</param>
        protected virtual void BindServices(CompositionBatch batch)
        {
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue<ILanguageManager>(new LanguageManager());
            batch.AddExportedValue(EnvironmentVariables);
            batch.AddExportedValue(this);
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Override to configure the framework and setup your IoC container.
        /// </summary>
        protected override void Configure()
        {
            // Add all assemblies to AssemblySource (using a temporary DirectoryCatalog).
            var directoryCatalog = new DirectoryCatalog(@".");
            AssemblySource.Instance.AddRange(
                directoryCatalog.Parts
                    .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                    .Where(assembly => !AssemblySource.Instance.Contains(assembly)));

            _priorityAssemblies = SelectAssemblies().ToList();
            var priorityCatalog = new AggregateCatalog(_priorityAssemblies.Select(x => new AssemblyCatalog(x)));
            var priorityProvider = new CatalogExportProvider(priorityCatalog);

            var mainCatalog = new AggregateCatalog(
                AssemblySource.Instance
                    .Where(assembly => !_priorityAssemblies.Contains(assembly))
                    .Select(x => new AssemblyCatalog(x)));
            var mainProvider = new CatalogExportProvider(mainCatalog);

            _container = new CompositionContainer(priorityProvider, mainProvider);
            priorityProvider.SourceProvider = _container;
            mainProvider.SourceProvider = _container;

            var batch = new CompositionBatch();

            BindServices(batch);

            _container.Compose(batch);
        }

        /// <summary>
        /// Clears the URL zones of all .dll files inside a given directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        private static void ClearUrlZonesInDirectory(string directoryPath)
        {
            foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*.dll", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(filePath);
                fileInfo.DeleteAlternateDataStream("Zone.Identifier");
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets all instances.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="T:System.Exception"></exception>
        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract).ToArray();
            if (exports.Any())
                return exports.First();
            throw new Exception($"Could not locate any instances of contract {contract}.");
        }
    }
}
