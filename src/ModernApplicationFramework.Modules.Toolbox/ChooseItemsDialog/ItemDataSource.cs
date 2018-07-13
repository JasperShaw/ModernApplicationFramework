using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Extended.Annotations;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public class ItemDataSource : INotifyPropertyChanged, IEquatable<ItemDataSource>
    {
        private bool _isChecked;
        private bool _isVisible = true;
        public event PropertyChangedEventHandler PropertyChanged;

        public ToolboxItemDefinitionBase Definition { get; }

        public string Name { get; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public string AssemblyName { get; }

        public string AssemblyVersion { get; }

        public string Namespace { get; }

        public IEnumerable<string> SearchableStrings { get; protected set; }

        public ItemDataSource(ToolboxItemDefinitionBase definition)
        {
            Definition = definition;
            Name = definition.Name;

            AssemblyName = Assembly.GetAssembly(definition.GetType()).GetName().Name;
            AssemblyVersion = Assembly.GetAssembly(definition.GetType()).GetName().Version.ToString();
            Namespace = definition.GetType().Namespace;

            SearchableStrings = new List<string> {Name};
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(ItemDataSource other)
        {
            if (other == null)
                return false;
            return Name == other.Name && AssemblyName == other.AssemblyName && Namespace == other.Namespace &&
                   AssemblyVersion == other.AssemblyVersion;
        }
    }
}
