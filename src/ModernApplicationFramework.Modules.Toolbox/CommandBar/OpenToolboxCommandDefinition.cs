using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    [Export(typeof(CommandDefinitionBase))]
    public class OpenToolboxCommandDefinition : CommandDefinition
    {
        private readonly IDockingHostViewModel _hostViewModel;
        public override string NameUnlocalized => "Toolbox";
        public override string Text => "Toolbox";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{D3CD6E1A-D2E9-4EDF-A83E-FB0B110BCA7F}");
        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override ICommand Command { get; }

        [ImportingConstructor]
        public OpenToolboxCommandDefinition(IDockingHostViewModel hostViewModel)
        {
            _hostViewModel = hostViewModel;
            var command = new UICommand(OpenToolbox, () => true);
            Command = command;
            DefaultKeyGesture = new MultiKeyGesture(new List<KeySequence>
            {
                new KeySequence(ModifierKeys.Control, Key.W),
                new KeySequence(Key.X)
            });
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        private void OpenToolbox()
        {
            _hostViewModel.ShowTool<IToolbox>();
        }
    }
}
