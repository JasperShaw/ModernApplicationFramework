using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Layout;
using DefinitionBase = ModernApplicationFramework.Basics.Definitions.Command.DefinitionBase;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(DefinitionBase))]
    public sealed class NewHorizontalTabGroupCommandDefinition : CommandDefinition
    {
        public NewHorizontalTabGroupCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(CreateNewHorizontalTabGroup, CanCreateNewHorizontalTabGroup);
        }

        private bool CanCreateNewHorizontalTabGroup()
        {
            if (DockingManager.Instace?.Layout.ActiveContent == null)
                return false;
            var parentDocumentGroup = DockingManager.Instace?.Layout.ActiveContent.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = DockingManager.Instace?.Layout.ActiveContent.Parent as LayoutDocumentPane;
            return (parentDocumentGroup == null ||
                    parentDocumentGroup.ChildrenCount == 1 ||
                    parentDocumentGroup.Root.Manager.AllowMixedOrientation ||
                    parentDocumentGroup.Orientation == Orientation.Vertical) &&
                   parentDocumentPane != null &&
                   parentDocumentPane.ChildrenCount > 1;
        }

        private void CreateNewHorizontalTabGroup()
        {
            var layoutElement = DockingManager.Instace?.Layout.ActiveContent;
            if (layoutElement == null)
                return;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement?.Parent as LayoutDocumentPane;

            if (parentDocumentGroup == null)
            {
                if (parentDocumentPane != null)
                {
                    var grandParent = parentDocumentPane.Parent;
                    parentDocumentGroup = new LayoutDocumentPaneGroup() { Orientation = Orientation.Vertical };
                    grandParent.ReplaceChild(parentDocumentPane, parentDocumentGroup);
                }
                parentDocumentGroup?.Children.Add(parentDocumentPane);
            }
            if (parentDocumentGroup != null)
            {
                parentDocumentGroup.Orientation = Orientation.Vertical;
                int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
                parentDocumentGroup.InsertChildAt(indexOfParentPane + 1, new LayoutDocumentPane(layoutElement));
            }
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        public override ICommand Command { get; }

        public override string Name => "New Horizontal Tab Group";
        public override string Text => "New Horizontal Tab Group";
        public override string ToolTip => null;
        public override Uri IconSource =>
            new Uri("/ModernApplicationFramework;component/Resources/Icons/SplitScreenHorizontal.xaml",
                UriKind.RelativeOrAbsolute);

        public override string IconId => "SplitScreenHorizontal";

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
    }
}
