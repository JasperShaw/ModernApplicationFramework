using System.Collections;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog.Internal
{
    [Export(typeof(InvisibleItemsDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class InvisibleItemsDialogViewModel : Screen
    {
        private IEnumerable _itemsSource;

        public IEnumerable ItemsSource
        {
            get => _itemsSource;
            set
            {
                if (Equals(value, _itemsSource))
                    return;
                _itemsSource = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
