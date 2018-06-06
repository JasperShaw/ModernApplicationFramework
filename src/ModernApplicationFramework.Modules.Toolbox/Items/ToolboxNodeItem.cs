﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Modules.Toolbox.Annotations;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    public abstract class ToolboxNodeItem : IToolboxNode
    {
        private bool _isExpanded;
        private bool _isInRenameMode;
        private bool _isSelected;
        private string _name;

        private string _renameBackup;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler CreatedCancelled;
        public event EventHandler Created;

        public Guid Id { get; }
        public bool IsCustom { get; protected set; }

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

        public bool IsInRenameMode
        {
            get => _isInRenameMode;
            set
            {
                if (value == _isInRenameMode) return;
                _isInRenameMode = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                if (!value)
                    ExitRenameMode();
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool IsNewlyCreated { get; protected set; }

        protected ToolboxNodeItem(Guid id, string name, bool isCustom = false)
        {
            Id = id;
            Name = name;
            IsCustom = isCustom;
        }

        public void CommitRename()
        {
            if (IsNewlyCreated)
                OnCreated();
            IsInRenameMode = false;
            IsNewlyCreated = false;
        }

        public void EnterRenameMode()
        {
            _renameBackup = _name;
            IsInRenameMode = true;
        }

        public void ExitRenameMode()
        {
            if (IsNewlyCreated)
            {
                OnCreatedCancelled();
                Name = string.Empty;
            }
            else
                Name = _renameBackup;
            IsNewlyCreated = false;
            IsInRenameMode = false;
        }

        public virtual bool IsRenameValid(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(Name))
            {
                errorMessage = "Name must not be empty";
                return false;
            }
            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnCreatedCancelled()
        {
            CreatedCancelled?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCreated()
        {
            Created?.Invoke(this, EventArgs.Empty);
        }
    }
}