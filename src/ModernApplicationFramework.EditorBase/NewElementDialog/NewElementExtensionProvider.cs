using System;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;

namespace ModernApplicationFramework.EditorBase.NewElementDialog
{
    public abstract class NewElementExtensionProvider : INewElementExtensionsProvider
    {
        private readonly Lazy<NewElementExtesionRootTreeNode> _rootNode;

        public abstract string Text { get; }

        public abstract uint SortOrder { get; }

        public INewElementExtensionTreeNode ExtensionsTree => _rootNode.Value;

        protected NewElementExtensionProvider()
        {
            _rootNode = new Lazy<NewElementExtesionRootTreeNode>(ConstructTree);
        }

        protected abstract NewElementExtesionRootTreeNode ConstructTree();
    }
}