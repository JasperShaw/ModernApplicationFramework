using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Modules.Toolbox.Annotations;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Resources;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    public abstract class ToolboxNode : IToolboxNode
    {
        private string _editingName;
        private bool _isExpanded;
        private bool _isInRenameMode;
        private string _name;

        public event EventHandler Created;

        public event EventHandler CreatedCancelled;

        public event PropertyChangedEventHandler PropertyChanged;

        public string EditingName
        {
            get => _editingName;
            set
            {
                if (value == _editingName) return;
                _editingName = value;
                OnPropertyChanged();
            }
        }

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

        public bool IsNewlyCreated { get; protected set; }

        public virtual bool IsNameModified { get; protected set; }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name)
                    return;
                _name = value;
                IsNameModified = true;
                OnPropertyChanged();
            }
        }

        protected string OriginalName { get; set; }

        protected ToolboxNode(Guid id, string name, bool isCustom = false)
        {
            Id = id;
            _name = name;
            IsCustom = isCustom;
        }

        public void CommitRename()
        {
            if (IsNewlyCreated)
                OnCreated();
            IsInRenameMode = false;
            IsNewlyCreated = false;
            Name = EditingName;
            _editingName = string.Empty;
        }

        public void EnterRenameMode()
        {
            EditingName = _name;
            IsInRenameMode = true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is IToolboxNode sequence)
                return Equals(sequence);
            return false;
        }

        public void ExitRenameMode()
        {
            if (IsNewlyCreated)
            {
                OnCreatedCancelled();
                Name = string.Empty;
            }

            IsNewlyCreated = false;
            IsInRenameMode = false;
        }

        public virtual bool IsRenameValid(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(EditingName))
            {
                errorMessage = ToolboxResources.Error_NameEmpty;
                return false;
            }

            return true;
        }

        public virtual void Reset()
        {
            IsNameModified = false;
            _name = OriginalName;
            OnPropertyChanged(nameof(Name));
        }

        protected virtual bool Equals(IToolboxNode other)
        {
            if (other.Id.Equals(Id) && other.Name.Equals(Name))
                return true;
            return false;
        }

        protected virtual void OnCreated()
        {
            Created?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCreatedCancelled()
        {
            CreatedCancelled?.Invoke(this, EventArgs.Empty);
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}