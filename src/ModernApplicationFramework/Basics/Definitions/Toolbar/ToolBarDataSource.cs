﻿using System;
using System.Globalization;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.Toolbar
{
    /// <inheritdoc cref="CommandBarDataSource" />
    /// <summary>
    /// A command bar definition that specifies a tool bar
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IHasInternalName" />
    public sealed class ToolBarDataSource : CommandBarDataSource, IHasInternalName
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

        public ToolbarScope ToolbarScope { get; }

        public override Guid Id { get; }

        public ToolBarDataSource(Guid id, string text, uint sortOrder, bool isCustom, Dock position, 
            ToolbarScope scope = ToolbarScope.MainWindow, CommandBarFlags flags = CommandBarFlags.CommandFlagNone) : 
            base(text, sortOrder, new ToolbarCommandDefinition(), isCustom, false, flags)
        {
            Id = id;
            _position = position;
            ToolbarScope = scope;
            _internalName = new AccessKeyRemovingConverter()
                .Convert(text, typeof(string), null, CultureInfo.CurrentCulture)
                ?.ToString();
        }

        private sealed class ToolbarCommandDefinition : CommandDefinitionBase
        {
            public override string Name => null;
            public override string NameUnlocalized => null;
            public override string Text => null;
            public override string ToolTip => null;
            public override bool IsList => false;
            public override CommandCategory Category => null;

            public override CommandControlTypes ControlType => CommandControlTypes.Menu;
            public override Guid Id => new Guid("{18C535DB-1E23-4B27-9BB9-A38F0BC6E036}");
        }
    }

    public enum ToolbarScope
    {
        MainWindow,
        Anchorable
    }
}