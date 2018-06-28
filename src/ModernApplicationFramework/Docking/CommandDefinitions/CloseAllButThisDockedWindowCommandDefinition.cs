using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(CloseAllButThisDockedWindowCommandDefinition))]
    public sealed class CloseAllButThisDockedWindowCommandDefinition : CommandDefinition<ICloseAllButThisDockedWindowCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("CloseAllButThisDockedWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.CloseAllButThisDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{D323EFFE-7A78-40FB-A1A8-393B4010D848}");
    }

    public interface ICloseAllButThisDockedWindowCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(ICloseAllButThisDockedWindowCommand))]
    internal class CloseAllButThisDockedWindowCommand : CommandDefinitionCommand, ICloseAllButThisDockedWindowCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            if (DockingManager.Instance == null)
                return false;
            var root = DockingManager.Instance.Layout.ActiveContent?.Root;
            if (root == null)
                return false;

            if (!DockingManager.Instance.Layout.ActiveContent.Root.Manager.CanCloseAllButThis)
                return false;

            return DockingManager.Instance.Layout.ActiveContent.Root.Manager.Layout.Descendents()
                .OfType<LayoutContent>()
                .Any(
                    d =>
                        // I know this does not make much sense but VS behaves like this...
                        //!Equals(d, LayoutElement) &&
                        d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow);
        }

        protected override void OnExecute(object parameter)
        {
            var dm = DockingManager.Instance?.Layout.ActiveContent;
            DockingManager.Instance?._ExecuteCloseAllButThisCommand(dm);
        }
    }
}