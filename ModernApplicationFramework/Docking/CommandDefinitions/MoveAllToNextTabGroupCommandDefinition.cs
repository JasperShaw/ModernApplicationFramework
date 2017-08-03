using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(MoveAllToNextTabGroupCommandDefinition))]
    public sealed class MoveAllToNextTabGroupCommandDefinition : CommandDefinition
    {
        public override UICommand Command { get; }
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => Text;
        public override string Text => DockingResources.MoveAllToNextTabGroupCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public MoveAllToNextTabGroupCommandDefinition()
        {
            Command = new UICommand(MoveAllToNextTabGroup, CanAllMoveToNextTabGroup);
        }

        private bool CanAllMoveToNextTabGroup()
        {
            if (DockingManager.Instace?.Layout.ActiveContent == null)
                return false;

            var parentDocumentGroup = DockingManager.Instace?.Layout.ActiveContent
                .FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = DockingManager.Instace?.Layout.ActiveContent.Parent as LayoutDocumentPane;
            return parentDocumentGroup != null &&
                   parentDocumentPane != null &&
                   parentDocumentGroup.ChildrenCount > 1 &&
                   parentDocumentGroup.IndexOfChild(parentDocumentPane) < parentDocumentGroup.ChildrenCount - 1 &&
                   parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) + 1] is
                       LayoutDocumentPane &&
                   parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane)].ChildrenCount > 1;
        }

        private void MoveAllToNextTabGroup()
        {
            var layoutElement = DockingManager.Instace?.Layout.ActiveContent;
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