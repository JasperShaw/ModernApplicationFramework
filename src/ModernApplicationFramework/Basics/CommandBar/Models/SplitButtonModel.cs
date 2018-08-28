using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Models
{
    public sealed class SplitButtonModel
    {
        public ObservableCollection<IHasTextProperty> Items { get; }

        public IStatusStringCreator StatusStringCreator { get; }

        public SplitButtonModel(IStatusStringCreator statusStringCreator) : this(new List<IHasTextProperty>(), statusStringCreator)
        {
        }

        public SplitButtonModel(IEnumerable<IHasTextProperty> itemsSource, IStatusStringCreator statusStringCreator)
        {
            StatusStringCreator = statusStringCreator;
            if (itemsSource is ObservableCollection<IHasTextProperty> observable)
            {
                Items = observable;
            }
            else
                Items = new BindableCollection<IHasTextProperty>(itemsSource);
        }
    }
}