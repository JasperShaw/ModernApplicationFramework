using System;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    /// <inheritdoc />
    /// <summary>
    /// A command bar definition that specifies a menu bar
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    public sealed class MenuBarDataSource : CommandBarDataSource
    {
        private string _internalName;

        /// <summary>
        /// Localized internal name of the menu bar
        /// </summary>
        public string InternalName
        {
            get => _internalName;
            set
            {
                if (value == _internalName) return;
                _internalName = value;
                OnPropertyChanged();
            }
        }

        public override Guid Id { get; }

        public MenuBarDataSource(Guid id, string text, uint sortOrder) : base(text, sortOrder, null, false, false)
        {
            Id = id;
            _internalName = text;
        }
    }
}