using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Output
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class OpenOutputToolCommandDefinition : CommandDefinition<IOpenOutputToolCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override ImageMoniker ImageMonikerSource => Monikers.Output;

        public override string Name => "View.Output";
        public override string NameUnlocalized => "Output";
        public override string Text => "Output";
        public override string ToolTip => "Output";

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{ED3DC8E1-F15B-4DBD-8C8E-272194C0642D}");

        public OpenOutputToolCommandDefinition()
        {
            DefaultKeyGestures = new []{ new MultiKeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Alt)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }

    public interface IOpenOutputToolCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IOpenOutputToolCommand))]
    internal class OpenOutputToolCommand : CommandDefinitionCommand, IOpenOutputToolCommand
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
            _shell.DockingHost.ShowTool<IOutput>();
        }
    }
}