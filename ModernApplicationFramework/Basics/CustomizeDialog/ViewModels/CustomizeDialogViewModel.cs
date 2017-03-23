﻿using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    [Export(typeof(CustomizeDialogViewModel))]
    internal sealed class CustomizeDialogViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public CustomizeDialogViewModel()
        {
            DisplayName = "Customize";
            var toolbarsViewModel = IoC.Get<IToolBarsPageViewModel>();
            ActivateItem(toolbarsViewModel);
        }


        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }
    }
}