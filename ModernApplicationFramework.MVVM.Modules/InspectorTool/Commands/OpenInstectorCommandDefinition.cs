using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Commands
{
    [Export(typeof(DefinitionBase))]
    public sealed class OpenInstectorCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get; }

        public override string IconId => "PropertyIcon";

        public override Uri IconSource =>
            new Uri("/ModernApplicationFramework.MVVM.Modules;component/Resources/Icons/Property_16x.xaml",
                UriKind.RelativeOrAbsolute);

        public override string Name => "Inspector";
        public override string Text => Name;
        public override string ToolTip => Name;

        public string MyText { get; set; }

        public OpenInstectorCommandDefinition()
        {
            Command = new GestureCommandWrapper(Open, CanOpen, new KeyGesture(Key.F4));
        }

        private bool CanOpen()
        {
            return _shell != null;
        }

        private void Open()
        {
            _shell.DockingHost.ShowTool<IInspectorTool>();
        }
    }
}