using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    [Export(typeof(ChooseItemsDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ChooseItemsDialogViewModel : Conductor<IScreen>.Collection.AllActive
    {
        public ICommand OkCommand => new Command(OnOk);
        public ICommand CancelCommand => new Command(OnCancel);
        public ICommand ResetCommand => new Command(OnReset);

        [ImportingConstructor]
        public ChooseItemsDialogViewModel([ImportMany] IEnumerable<IToolboxItemPage> pages)
        {
            Items.AddRange(pages);
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            if (!(view is Window window))
                throw new InvalidOperationException("View is not a window");

            window.Closing += Window_Closing;
            window.Closed += Window_Closed;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void OnOk()
        {
        }

        private void OnCancel()
        {
        }

        private void OnReset()
        {
        }
    }
}
