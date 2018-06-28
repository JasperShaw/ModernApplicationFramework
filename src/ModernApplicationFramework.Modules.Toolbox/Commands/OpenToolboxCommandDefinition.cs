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

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public class OpenToolboxCommandDefinition : CommandDefinition<IOpenToolboxCommand>
    {
        public override string NameUnlocalized => "Toolbox";
        public override string Text => "Toolbox";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{D3CD6E1A-D2E9-4EDF-A83E-FB0B110BCA7F}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public OpenToolboxCommandDefinition()
        {
            DefaultKeyGestures = new[]
            {
                new MultiKeyGesture(new List<KeySequence>
                {
                    new KeySequence(ModifierKeys.Control, Key.W),
                    new KeySequence(Key.X)
                })
            };
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }

    public interface IOpenToolboxCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IOpenToolboxCommand))]
    internal class OpenToolboxCommand : CommandDefinitionCommand, IOpenToolboxCommand
    {
        private readonly IDockingHostViewModel _hostViewModel;

        [ImportingConstructor]
        public OpenToolboxCommand(IDockingHostViewModel hostViewModel)
        {
            _hostViewModel = hostViewModel;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            _hostViewModel.ShowTool<IToolbox>();
        }
    }
}
