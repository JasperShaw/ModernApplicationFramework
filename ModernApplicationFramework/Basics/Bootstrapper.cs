using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.ApplicationEnvironment;
using ModernApplicationFramework.Core.Localization;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Native.TrinetCoreNtfs;

namespace ModernApplicationFramework.Basics
{
    public class Bootstrapper : BootstrapperBase
    {
        protected CompositionContainer Container;

        protected List<Assembly> _priorityAssemblies;


        protected virtual IEnvironmentVarirables EnvironmentVarirables => new FallbackEnvironmentVarirables();


        public Bootstrapper()
        {
			PreInitialize();
            Initialize();
	        SetLanguage();
        }

	    internal IList<Assembly> PriorityAssemblies => _priorityAssemblies;

	    protected virtual void PreInitialize()
	    {
		    
	    }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            IoC.Get<IEnvironmentVarirables>().Setup();
            IoC.Get<IApplicationEnvironment>().Setup();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
            IoC.Get<IApplicationEnvironment>().PrepareClose();
        }

        protected void SetLanguage()
        {
	        var lm = Container.GetExportedValue<ILanguageManager>() as LanguageManager;
			lm?.SetLanguage();
        }

        protected virtual void BindServices(CompositionBatch batch)
        {
            batch.AddExportedValue(EnvironmentVarirables);

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue<ILanguageManager>(new LanguageManager());

            batch.AddExportedValue(new EnvironmentGeneralOptions());

            batch.AddExportedValue(Container);
            batch.AddExportedValue(this);
        }

        protected override void PrepareApplication()
        {
            base.PrepareApplication();
            var exassemlby = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(exassemlby);
            //This makes sure we can share any applications through the Internet
            ClearUrlZonesInDirectory(directory);
        }

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

            Container = new CompositionContainer(priorityProvider, mainProvider);
            priorityProvider.SourceProvider = Container;
            mainProvider.SourceProvider = Container;

            var batch = new CompositionBatch();

            BindServices(batch);
            batch.AddExportedValue(mainCatalog);

            Container.Compose(batch);
        }

        protected static void ClearUrlZonesInDirectory(string directoryPath)
        {
            foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*.dll", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(filePath);
                fileInfo.DeleteAlternateDataStream("Zone.Identifier");
            }
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = Container.GetExportedValues<object>(contract);

            var enumerable = exports as object[] ?? exports.ToArray();
            if (enumerable.Any())
                return enumerable.First();
            throw new Exception($"Could not locate any instances of contract {contract}.");
        }
    }
}
