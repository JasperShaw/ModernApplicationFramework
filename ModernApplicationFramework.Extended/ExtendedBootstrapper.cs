using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Extended.ApplicationEnvironment;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended
{
    public class ExtendedBootstrapper : Basics.Bootstrapper
    {
        protected virtual bool UseSettingsManager => true;

        protected new virtual IExtendedEnvironmentVarirables EnvironmentVarirables => new FallbackExtendedEnvironmentVarirables();

        internal IList<Assembly> PriorityAssemblies => _priorityAssemblies;

        protected override void BindServices(CompositionBatch batch)
        {
            batch.AddExportedValue(EnvironmentVarirables);
            batch.AddExportedValue<IEnvironmentVarirables>(EnvironmentVarirables);
            base.BindServices(batch);
	     	batch.AddExportedValue(this);
        }

        protected override void BuildUp(object instance)
        {
            Container.SatisfyImportsOnce(instance);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            try
            {
                var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
                var exports = Container.GetExportedValues<object>(contract);

                var enumerable = exports as object[] ?? exports.ToArray();
                if (enumerable.Any())
                    return enumerable.First();
                throw new Exception($"Could not locate any instances of contract {contract}.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.UnwrapCompositionException().Message);
            }
            return null;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            IoC.Get<IExtendedEnvironmentVarirables>().Setup();
            IoC.Get<IApplicationEnvironment>().Setup();
            DisplayRootViewFor<IDockingMainWindowViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
            IoC.Get<IApplicationEnvironment>().PrepareClose();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
            {
                Assembly.GetEntryAssembly()
            };
        }
    }
}