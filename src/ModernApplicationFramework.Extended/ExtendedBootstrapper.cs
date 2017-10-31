using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Extended.ApplicationEnvironment;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Extended
{
    public abstract class ExtendedBootstrapper : Bootstrapper
    {
        protected virtual bool UseSettingsManager => true;

        protected new virtual IExtendedEnvironmentVariables EnvironmentVariables => new FallbackExtendedEnvironmentVariables();

        protected override void BindServices(CompositionBatch batch)
        {
            batch.AddExportedValue(EnvironmentVariables);
            batch.AddExportedValue<IEnvironmentVariables>(EnvironmentVariables);
            base.BindServices(batch);
            batch.AddExportedValue(this);
        }
        
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            IoC.Get<IExtendedEnvironmentVariables>().Setup();
            IoC.Get<IApplicationEnvironment>().Setup();
            IoC.Get<IKeyBindingManager>();
            DisplayRootViewFor<IDockingMainWindowViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            IoC.Get<IApplicationEnvironment>().PrepareClose();
            base.OnExit(sender, e);
        }
    }
}