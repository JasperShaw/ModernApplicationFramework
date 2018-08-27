using System;
using System.Globalization;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc cref="CommandBarDataSource" />
    /// <summary>
    /// A command bar definition that specifies a tool bar
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IHasInternalName" />
    internal sealed class ToolBarDataSource : CommandBarDataSource, IHasInternalName
    {
        private Dock _position;
        private string _internalName;
        private int _placementSlot;


        /// <summary>
        /// The current dock position of the tool bar
        /// </summary>
        public Dock Position
        {
            get => _position;
            set
            {
                if (value == _position)
                    return;
                LastPosition = _position;
                _position = value;
                OnPropertyChanged();
            }
        }

        public int PlacementSlot
        {
            get => _placementSlot;
            set
            {
                if (value == _placementSlot)
                    return;
                _placementSlot = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The last know dock position
        /// </summary>
        public Dock LastPosition { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// The unlocalized internal name of the object
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

        public override CommandControlTypes UiType => CommandControlTypes.Toolbar;

        public ToolbarScope ToolbarScope { get; }

        public override Guid Id { get; }

        public ToolBarDataSource(Guid id, string text, uint sortOrder, bool isCustom, Dock position, 
            ToolbarScope scope = ToolbarScope.MainWindow, CommandBarFlags flags = CommandBarFlags.CommandFlagNone) : 
            base(text, sortOrder, null, isCustom, false, flags)
        {
            Id = id;
            _position = position;
            ToolbarScope = scope;
            _internalName = new AccessKeyRemovingConverter()
                .Convert(text, typeof(string), null, CultureInfo.CurrentCulture)
                ?.ToString();
        }
    }

    public enum ToolbarScope
    {
        MainWindow,
        Anchorable
    }
}