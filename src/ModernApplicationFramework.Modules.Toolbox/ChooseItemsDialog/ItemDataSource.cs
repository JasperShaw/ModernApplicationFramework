using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Extended.Annotations;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    /// <summary>
    /// Item model for the toolbox item choose dialog
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    /// <seealso cref="!:System.IEquatable{ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog.ItemDataSource}" />
    public class ItemDataSource : INotifyPropertyChanged, IEquatable<ItemDataSource>
    {
        private bool _isChecked;
        private bool _isVisible = true;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The name of the assembly of the <see cref="Definition"/>.
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// The assembly version of the <see cref="Definition"/>.
        /// </summary>
        public string AssemblyVersion { get; }

        /// <summary>
        /// The definition data.
        /// </summary>
        public ToolboxItemDefinitionBase Definition { get; }

        /// <summary>
        /// Gets or sets a value indicating whether item is checked.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether item is visible.
        /// </summary>
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

        /// <summary>
        /// The displayed name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The namespace of the <see cref="Definition"/>.
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// Gets or sets searchable strings patterns.
        /// </summary>
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

        public bool Equals(ItemDataSource other)
        {
            if (other == null)
                return false;
            return Name == other.Name && AssemblyName == other.AssemblyName && Namespace == other.Namespace &&
                   AssemblyVersion == other.AssemblyVersion;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}