using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(NewVerticalTabGroupCommandDefinition))]
    public sealed class NewVerticalTabGroupCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("NewVerticalTabGroupCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.NewVerticalTabGroupCommandDefinition_Text;
        public override string ToolTip => null;

        public override Uri IconSource =>
            new Uri("/ModernApplicationFramework;component/Resources/Icons/SplitScreenVertical.xaml",
                UriKind.RelativeOrAbsolute);

        public override string IconId => "SplitScreenVertical";
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{5667C276-A91F-428A-86A3-7D95814B4B9F}");

        public NewVerticalTabGroupCommandDefinition()
        {
            Command = new UICommand(CreateNewVerticalTabGroup, CanCreateNewVerticalTabGroup);
        }

        private bool CanCreateNewVerticalTabGroup()
        {
            if (DockingManager.Instance?.Layout.ActiveContent == null)
                return false;
            var parentDocumentGroup = DockingManager.Instance?.Layout.ActiveContent
                .FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = DockingManager.Instance?.Layout.ActiveContent.Parent as LayoutDocumentPane;
            return (parentDocumentGroup == null ||
                    parentDocumentGroup.ChildrenCount == 1 ||
                    parentDocumentGroup.Root.Manager.AllowMixedOrientation ||
                    parentDocumentGroup.Orientation == Orientation.Horizontal) &&
                   parentDocumentPane != null &&
                   parentDocumentPane.ChildrenCount > 1;
        }

        private void CreateNewVerticalTabGroup()
        {
            var layoutElement = DockingManager.Instance?.Layout.ActiveContent;
            if (layoutElement == null)
                return;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;

            if (parentDocumentGroup == null)
            {
                if (parentDocumentPane != null)
                {
                    var grandParent = parentDocumentPane.Parent;
                    parentDocumentGroup = new LayoutDocumentPaneGroup {Orientation = Orientation.Horizontal};
                    grandParent.ReplaceChild(parentDocumentPane, parentDocumentGroup);
                }
                parentDocumentGroup?.Children.Add(parentDocumentPane);
            }
            if (parentDocumentGroup != null)
            {
                parentDocumentGroup.Orientation = Orientation.Horizontal;
                var indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
                parentDocumentGroup.InsertChildAt(indexOfParentPane + 1, new LayoutDocumentPane(layoutElement));
            }
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }
    }
}