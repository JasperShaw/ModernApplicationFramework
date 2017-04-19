using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.ModuleBase;
using ModernApplicationFramework.Extended.Modules.InspectorTool;
using ModernApplicationFramework.Extended.Modules.OutputTool;

namespace ModernApplicationFramework.MVVM.Demo.Modules.ComboBoxMenuTest
{
    [Export(typeof(IModule))]
    [Export(typeof(ComboBoxTestModule))]
    public class ComboBoxTestModule : ModuleBase, INotifyPropertyChanged
    {
        private object _item;

        [ImportingConstructor]
        public ComboBoxTestModule(IOutput output, IInspectorTool inspectorTool)
        {
            Items = new List<object> {"Test", "Test1", "Test2", "Test3"};
        }

        public IList<object> Items { get;  }

        public object Item
        {
            get => _item;
            set
            {
                if (Equals(value, _item)) return;
                _item = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
