using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Command;
using MordernApplicationFramework.WindowManagement.LayoutManagement;

namespace MordernApplicationFramework.WindowManagement.Commands
{
    /// <summary>
    /// Implementation to list all tool bars as a menu item. By clicking on the menu item the tool bars visibility will be toggled
    /// </summary>
    /// <seealso cref="ICommandListHandler{TCommandDefinition}" />
    [Export(typeof(ICommandHandler))]
    public class ShowLayoutsCommandHandler : ICommandListHandler<ListWindowLayoutsCommandDefinition>
    {
        private readonly ILayoutManager _layoutManager;
        private readonly IWindowLayoutStore _layoutStore;

        [ImportingConstructor]
        internal ShowLayoutsCommandHandler(ILayoutManager layoutManager, IWindowLayoutStore layoutStore)
        {
            _layoutManager = layoutManager;
            _layoutStore = layoutStore;
        }

        public void Populate(Command command, List<CommandDefinitionBase> commands)
        {
            var layoutPairs  = LayoutManagementUtilities.EnumerateLayoutKeyInfo(_layoutStore);
            var layouts = new ObservableCollection<LayoutItemViewModel>(layoutPairs
                .Select(kvp => new LayoutItemViewModel(kvp.Key, kvp.Value)).OrderBy(lvm => lvm.Position)
                .ThenBy(lvm => lvm.Name));
            if (layouts.Count == 0)
            {
                return;
            }
            commands.AddRange(layouts.Select(layout => new ShowLayoutCommandDefinition(layout.Name)
            {
                CommandParamenter = layout.Info
            }));
        }

        private class ShowLayoutCommandDefinition : CommandDefinition
        {
            public override UICommand Command { get; }
            public override MultiKeyGesture DefaultKeyGesture => null;
            public override GestureScope DefaultGestureScope => null;
            public override string Name => string.Empty;
            public override string NameUnlocalized => string.Empty;
            public override string Text { get; }
            public override string ToolTip => string.Empty;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override CommandCategory Category => null;

            public ShowLayoutCommandDefinition(string name)
            {
                Text = name;
                Command = new UICommand(ShowLayout, CanShowLayout);
            }

            private bool CanShowLayout()
            {
                return CommandParamenter is WindowLayoutInfo;
            }

            private void ShowLayout()
            {
                //var toolBarDef = CommandParamenter as WindowLayoutInfo;
            }
        }
    }
}