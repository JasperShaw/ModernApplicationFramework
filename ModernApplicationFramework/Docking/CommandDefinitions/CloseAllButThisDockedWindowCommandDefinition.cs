﻿using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(DefinitionBase))]
    public sealed class CloseAllButThisDockedWindowCommandDefinition : CommandDefinition
    {
        public CloseAllButThisDockedWindowCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(CloseAllButThisDockedWindows, CanCloseAllButThisDockedWindows);
        }

        private bool CanCloseAllButThisDockedWindows()
        {
            if (DockingManager.Instace == null)
                return false;
            var root = DockingManager.Instace.Layout.ActiveContent?.Root;
            if (root == null)
                return false;

            if (!DockingManager.Instace.Layout.ActiveContent.Root.Manager.CanCloseAllButThis)
                return false;

            return DockingManager.Instace.Layout.ActiveContent.Root.Manager.Layout.
                Descendents()
                .OfType<LayoutContent>()
                .Any(
                    d =>
                        // I know this does not make much sense but VS behaves like this...
                        //!Equals(d, LayoutElement) &&
                            d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow);
        }

        private void CloseAllButThisDockedWindows()
        {
            var dm = DockingManager.Instace?.Layout.ActiveContent;
            DockingManager.Instace?._ExecuteCloseAllButThisCommand(dm);
        }

        public override ICommand Command { get; }

        public override string Name => "Close All But This";
        public override string Text => "Close All But This";
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.FileCommandCategory;
    }
}
