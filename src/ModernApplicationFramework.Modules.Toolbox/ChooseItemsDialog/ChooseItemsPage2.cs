using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Extended.Annotations;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    [Export(typeof(IToolboxItemPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class ChooseItemsPage1 : ChooseItemsPage
    {
        [ImportingConstructor]
        public ChooseItemsPage1([ImportMany] IEnumerable<ToolboxItemDefinitionBase> definitions)
        {
            DisplayName = "Test1";

            foreach (var definition in definitions)
                Items.Add(new TestData(definition.Name, definition.Data.Format));

        }

        protected override IEnumerable<HeaderInformation> GetColumns()
        {
            var list = new List<HeaderInformation>
            {
                new HeaderInformation("Some Text", nameof(TestData.Name)),
                new HeaderInformation("Some Text 2", nameof(TestData.Format))
            };
            return list;
        }
    }

    public class TestData : IItemInfo
    {
        private bool _isChecked;
        public string Name { get; }
        public string Version { get; }
        public string Assembly { get; }

        public string Format { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value == _isChecked)
                    return;
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public TestData(string name, string format)
        {
            Format = format;
            Name = name;

            Assembly = GetType().Assembly.GetName().Name;
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
      
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
