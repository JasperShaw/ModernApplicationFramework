using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;

namespace ModernApplicationFramework.EditorBase.Dialogs.NewElementDialog
{
    public class NewElementExtensionTreeNode : INewElementExtensionTreeNode, INotifyPropertyChanged
    {
        private readonly ObservableCollection<IExtensionDefinition> _extensions;

        public string Text { get; }

        public IList<INewElementExtensionTreeNode> Nodes => null;

        public INewElementExtensionTreeNode Parent { get; }
        public IList<IExtensionDefinition> Extensions => _extensions;

        public bool IsSelected { get; set; }

        public bool IsExpanded { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public NewElementExtensionTreeNode(INewElementExtensionTreeNode parent, string text, IEnumerable<IExtensionDefinition> extensions)
        {
            Parent = parent;
            Text = text;
            if (extensions == null)
                throw new ArgumentNullException("Extensions cannot be null");
            _extensions = new ObservableCollection<IExtensionDefinition>();
            foreach (var extension in extensions)
                AddItemToExtensionsList(extension);
        }

        private void AddItemToExtensionsList(IExtensionDefinition extension)
        {
            if (extension == null)
                return;
            _extensions.Add(extension);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}