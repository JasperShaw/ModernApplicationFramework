using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Inspector.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class OpenInspectorCommandDefinition : CommandDefinition<IOpenInspectorCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override string IconId => "PropertyIcon";

        public override Uri IconSource =>
            new Uri("/ModernApplicationFramework.Modules.Inspector;component/Resources/Icons/Property_16x.xaml",
                UriKind.RelativeOrAbsolute);

        public override string Name => "View.Inspector";
        public override string Text => "Inspector";
        public override string NameUnlocalized => "Inspector";
        public override string ToolTip => "Inspector";

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{A948FC05-72EF-4309-BF54-E697F42C32D1}");

        public OpenInspectorCommandDefinition()
        {
            DefaultKeyGestures = new []{new MultiKeyGesture(Key.F4)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }

    public interface IOpenInspectorCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IOpenInspectorCommand))]
    internal class OpenInspectorCommand : CommandDefinitionCommand, IOpenInspectorCommand
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        protected override bool OnCanExecute(object parameter)
        {
            return _shell != null;
        }

        protected override void OnExecute(object parameter)
        {
            _shell.DockingHost.ShowTool<IInspectorTool>();
        }
    }
}