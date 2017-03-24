using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Properties;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFramework.Basics.Definitions.Toolbar
{
    public class ToolbarDefinitionOld : INotifyPropertyChanged
    {
        private bool _visible;
        private Dock _position;
        private string _name;

        public ToolbarDefinitionOld(ToolBar toolBar, string name, int sortOrder, bool visible, Dock position, bool isCustom = false)
        {
            ToolBar = toolBar;
            Name = name;
            SortOrder = sortOrder;
            Visible = visible;
            Position = position;
            IsCustom = isCustom;

            Items = new List<string>
            {
                "Test", "Test2"
            };
        }

        public string Name
        {
            get => _name;
            set
            {
                if (string.Equals(_name, value, StringComparison.Ordinal))
                    return;
                _name = value; 
                OnPropertyChanged();            
            }
        }

        public bool IsCustom { get; set; }

        public Dock LastPosition { get; private set; }

        public IEnumerable Items { get; set; }

        public Dock Position
        {
            get => _position;
            set
            {
                if (Equals(value, _position))
                    return;
                LastPosition = _position;
                _position = value;
                OnPropertyChanged();         
            }
        }

        public int SortOrder { get; set; }

        public bool Visible
        {
            get => _visible;
            set
            {
                if (Equals(value, _visible))
                    return;
                _visible = value;
                OnPropertyChanged();
            }
        }

        public ToolBar ToolBar { get; protected set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ToolbarDefinitionOld<T> : ToolbarDefinitionOld where T : ToolBar
    {
        public ToolbarDefinitionOld(string name, int sortOrder, bool visible, Dock position, bool isCustom = false)
            : base(null, name, sortOrder, visible, position, isCustom)
        {
            var t = IoC.Get<IToolbarService>().GetToolbar(typeof(T));
            ToolBar = t;
        }
    }
}