using System.ComponentModel.Composition;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.Commands
{
    [Export(typeof(IMoveAllToNextTabGroupCommand))]
    internal class MoveAllToNextTabGroupCommand : CommandDefinitionCommand, IMoveAllToNextTabGroupCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            if (DockingManager.Instance?.Layout.ActiveContent == null)
                return false;

            var parentDocumentGroup = DockingManager.Instance?.Layout.ActiveContent
                .FindParent<LayoutDocumentPaneGroup>();
            return parentDocumentGroup != null &&
                   DockingManager.Instance?.Layout.ActiveContent.Parent is LayoutDocumentPane parentDocumentPane &&
                   parentDocumentGroup.ChildrenCount > 1 &&
                   parentDocumentGroup.IndexOfChild(parentDocumentPane) < parentDocumentGroup.ChildrenCount - 1 &&
                   parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) + 1] is
                       LayoutDocumentPane &&
                   parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane)].ChildrenCount > 1;
        }

        protected override void OnExecute(object parameter)
        {
            var layoutElement = DockingManager.Instance?.Layout.ActiveContent;
            if (layoutElement == null)
                return;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;
            var indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            var nextDocumentPane = parentDocumentGroup.Children[indexOfParentPane + 1] as LayoutDocumentPane;

            if (parentDocumentPane != null)
                foreach (var layoutContent in parentDocumentPane.ChildrenSorted)
                {
                    if (Equals(layoutContent, layoutElement))
                        continue;
                    nextDocumentPane?.InsertChildAt(0, layoutContent);
                    layoutContent.Root.CollectGarbage();
                }
            nextDocumentPane?.InsertChildAt(0, layoutElement);
            layoutElement.IsActive = true;
            layoutElement.IsSelected = true;
            layoutElement.Root.CollectGarbage();
        }
    }
}