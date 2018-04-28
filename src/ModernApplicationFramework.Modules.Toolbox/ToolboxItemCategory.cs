﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Modules.Toolbox.Annotations;

namespace ModernApplicationFramework.Modules.Toolbox
{

    public abstract class ToolboxNodeItem : INotifyPropertyChanged
    {
        private bool _isExpanded;
        private bool _isSelected;
        public string Name { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value == _isExpanded) return;
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        protected ToolboxNodeItem(string name)
        {
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class ToolboxItemCategory : ToolboxNodeItem
    {
        public Type TargetType { get; }

        public ObservableCollection<IToolboxItem> Items { get; }

        public ToolboxItemCategory(Type targetType, string name) : base(name)
        {
            TargetType = targetType;
            Items = new ObservableCollection<IToolboxItem>();
        }
    }

    public interface IToolboxItem
    {
        ToolboxItemCategory Parent { get; }

        string Name { get; }

        Uri IconSource { get; set; }
    }

    public class ToolboxItemEx : ToolboxNodeItem, IToolboxItem
    {
        public Type TargetType { get; }
        public ToolboxItemCategory Parent { get; }
        public Uri IconSource { get; set; }

        public ToolboxItemEx(Type targetType, ToolboxItemCategory parent, string name, Uri iconSource = null) : base(name)
        {
            TargetType = targetType;
            Parent = parent;
            IconSource = iconSource;
            parent.Items?.Add(this);
        }
    }
}