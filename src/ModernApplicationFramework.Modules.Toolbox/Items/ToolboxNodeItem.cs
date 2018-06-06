using System;
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

        protected ToolboxNodeItem(Guid id, string name, bool isCustom = false)
        {
            Id = id;
            Name = name;
            IsCustom = isCustom;
        }

        public void CommitRename()
        {
            IsInRenameMode = false;
        }

        public void EnterRenameMode()
        {
            _renameBackup = _name;
            IsInRenameMode = true;
        }

        public void ExitRenameMode()
        {
            IsInRenameMode = false;
            Name = _renameBackup;
        }

        public virtual bool IsRenameValid(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (Name == _renameBackup)
                return true;
            if (!string.IsNullOrEmpty(Name))
                return true;
            errorMessage = "Name must not be empty";
            return false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}