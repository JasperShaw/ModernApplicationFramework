using System.Collections.Generic;

namespace ModernApplicationFramework.EditorBase.Interfaces.NewElement
{
    public interface INewElementExtensionTreeNode
    {
        string Text { get; }

        IList<INewElementExtensionTreeNode> Nodes { get; }

        INewElementExtensionTreeNode Parent { get; }

        IList<IExtensionDefinition> Extensions { get; }

        bool IsSelected { get; set; }

        bool IsExpanded { get; set; }
    }
}