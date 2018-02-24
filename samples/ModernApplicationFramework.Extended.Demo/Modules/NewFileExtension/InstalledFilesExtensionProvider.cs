using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.EditorBase.NewElementDialog;

namespace ModernApplicationFramework.Extended.Demo.Modules.NewFileExtension
{
    [Export(typeof(INewElementExtensionsProvider))]
    [Export(typeof(InstalledFilesExtensionProvider))]
    public class InstalledFilesExtensionProvider : NewElementExtensionProvider
    {
        public override string Text => "Installed";
        public override uint SortOrder => 0;

        protected override NewElementExtesionRootTreeNode ConstructTree()
        {
            var fileTypes = IoC.Get<IEditorProvider>().SupportedFileDefinitions;
            var rootNode = new NewElementExtesionRootTreeNode();
            var node = new NewElementExtensionTreeNode(rootNode, "All Files", fileTypes);
            rootNode.AddNode(node);
            var nodeEmpty = new NewElementExtensionTreeNode(rootNode, "Empty", new List<IExtensionDefinition>());
            rootNode.AddNode(nodeEmpty);
            return rootNode;
        }
    }
}
