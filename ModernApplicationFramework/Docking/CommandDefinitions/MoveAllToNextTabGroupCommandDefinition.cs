using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Layout;
using DefinitionBase = ModernApplicationFramework.Basics.Definitions.Command.DefinitionBase;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(DefinitionBase))]
    public sealed class MoveAllToNextTabGroupCommandDefinition : CommandDefinition
    {
        public MoveAllToNextTabGroupCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(MoveAllToNextTabGroup, CanAllMoveToNextTabGroup);
        }

        private bool CanAllMoveToNextTabGroup()
        {
            if (DockingManager.Instace?.Layout.ActiveContent == null)
                return false;

            var parentDocumentGroup = DockingManager.Instace?.Layout.ActiveContent.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = DockingManager.Instace?.Layout.ActiveContent.Parent as LayoutDocumentPane;
            return parentDocumentGroup != null &&
                   parentDocumentPane != null &&
                   parentDocumentGroup.ChildrenCount > 1 && 
                   parentDocumentGroup.IndexOfChild(parentDocumentPane) < parentDocumentGroup.ChildrenCount - 1 &&
                   parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) + 1] is
                       LayoutDocumentPane &&
                   parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) ].ChildrenCount > 1;
        }

        private void MoveAllToNextTabGroup()
        {
            var layoutElement = DockingManager.Instace?.Layout.ActiveContent;
            if (layoutElement == null)
                return;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;
            int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
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

        public override ICommand Command { get; }

        public override string Name => "Move All to Next Tab Group";
        public override string Text => "Move All to Next Tab Group";
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;
    }
}
