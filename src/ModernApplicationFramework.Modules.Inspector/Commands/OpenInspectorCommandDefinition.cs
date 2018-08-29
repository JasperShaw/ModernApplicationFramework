using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Inspector.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    public sealed class OpenInspectorCommandDefinition : CommandDefinition<IOpenInspectorCommand>
    {

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.F4))
        });

        public override ImageMoniker ImageMonikerSource => Monikers.Property;

        public override string Name => "View.Inspector";
        public override string Text => "Inspector";
        public override string NameUnlocalized => "Inspector";
        public override string ToolTip => "Inspector";

        public override CommandBarCategory Category => CommandBarCategories.ViewCategory;
        public override Guid Id => new Guid("{A948FC05-72EF-4309-BF54-E697F42C32D1}");
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