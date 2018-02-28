using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor;

namespace ModernApplicationFramework.EditorBase.Controls.EditorSelectorDialog
{
    public class EditorListItem : INotifyPropertyChanged
    {
        private bool _isDefault;
        private readonly string _editorName;
        private string _name;
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly string DefaultSuffix = EditorSelectorResources.DefaultSuffix;

        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                if (value == _isDefault) return;
                _isDefault = value;
                UpdateName(value);
                OnPropertyChanged();
            }
        }

        private void UpdateName(bool isDefault)
        {
            Name = isDefault ? _editorName + DefaultSuffix : _editorName;
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

        public IEditor Editor { get; }

        public EditorListItem(IEditor editor)
        {
            _editorName = editor.Name;
            Editor = editor;
            Name = editor.Name;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}