using System.Collections.Generic;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;

namespace ModernApplicationFramework.EditorBase.NewElementDialog
{
    public class NewElementExtesionRootTreeNode : INewElementExtensionTreeNode
    {
        private readonly List<INewElementExtensionTreeNode> _cildNodes;
        public string Text => string.Empty;
        public IList<INewElementExtensionTreeNode> Nodes => _cildNodes;
        public INewElementExtensionTreeNode Parent => null;
        public IList<IExtensionDefinition> Extensions => null;
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }

        public NewElementExtesionRootTreeNode()
        {
            _cildNodes = new List<INewElementExtensionTreeNode>();
        }

        public void AddNode(INewElementExtensionTreeNode node)
        {
            _cildNodes.Add(node);
        }
    }
}