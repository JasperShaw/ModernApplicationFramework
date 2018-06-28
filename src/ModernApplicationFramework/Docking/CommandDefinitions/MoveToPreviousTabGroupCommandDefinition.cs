using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(MoveToPreviousTabGroupCommandDefinition))]
    public sealed class MoveToPreviousTabGroupCommandDefinition : CommandDefinition<IMoveToPreviousTabGroupCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("MoveToPreviousTabGroupCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.MoveToPreviousTabGroupCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{8E11AADE-8B7D-44C1-815F-8D5D6C3C9644}");
    }

    public interface IMoveToPreviousTabGroupCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IMoveToPreviousTabGroupCommand))]
    internal class MoveToPreviousTabGroupCommand : CommandDefinitionCommand, IMoveToPreviousTabGroupCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            if (DockingManager.Instance?.Layout.ActiveContent == null)
                return false;
            var parentDocumentGroup = DockingManager.Instance?.Layout.ActiveContent
                .FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = DockingManager.Instance?.Layout.ActiveContent.Parent as LayoutDocumentPane;
            return parentDocumentGroup != null &&
                   parentDocumentPane != null &&
                   parentDocumentGroup.ChildrenCount > 1 &&
                   parentDocumentGroup.IndexOfChild(parentDocumentPane) > 0 &&
                   parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) - 1] is
                       LayoutDocumentPane;
        }

        protected override void OnExecute(object parameter)
        {
            var layoutElement = DockingManager.Instance?.Layout.ActiveContent;
            if (layoutElement == null)
                return;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;
            var indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            var nextDocumentPane = parentDocumentGroup.Children[indexOfParentPane - 1] as LayoutDocumentPane;
            nextDocumentPane?.InsertChildAt(0, layoutElement);
            layoutElement.IsActive = true;
            layoutElement.IsSelected = true;
            layoutElement.Root.CollectGarbage();
        }
    }
}