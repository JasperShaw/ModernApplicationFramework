using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Extended.ApplicationEnvironment;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.KeyBindingScheme;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Extended
{
    public class ExtendedBootstrapper : Bootstrapper
    {
        protected virtual bool UseSettingsManager => true;

        protected new virtual IExtendedEnvironmentVariables EnvironmentVariables => new FallbackExtendedEnvironmentVariables();

        internal IList<Assembly> PriorityAssemblies => _priorityAssemblies;

        protected override void BindServices(CompositionBatch batch)
        {
            batch.AddExportedValue(EnvironmentVariables);
            batch.AddExportedValue<IEnvironmentVariables>(EnvironmentVariables);
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
            IoC.Get<IExtendedEnvironmentVariables>().Setup();     
            IoC.Get<IApplicationEnvironment>().Setup();
            IoC.Get<KeyBindingManager>();
            DisplayRootViewFor<IDockingMainWindowViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            IoC.Get<IApplicationEnvironment>().PrepareClose();
            base.OnExit(sender, e);
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