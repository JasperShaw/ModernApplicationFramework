using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(SaveActiveFileCommandDefinition))]
    public class SaveActiveFileCommandDefinition : CommandDefinition<ISaveActiveFileCommand>
    {
        private string _text;

        public override string NameUnlocalized => "Save active file";
        public override string Text => BuildText();

        private string BuildText()
        {
            if (Command.CanExecute(null))
                return string.Format(CommandsResources.SaveActiveDocumentCommandText, _text);
            return CommandsResources.SaveActiveDocumentCommandName;
        }

        public override string ToolTip => Text;

        public override string Name => CommandsResources.SaveActiveDocumentCommandName;

        public override ImageMoniker ImageMonikerSource => Monikers.Save;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{651EA782-BFCB-4ACA-8F98-6798C117F988}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.S, ModifierKeys.Control))
        });

        [ImportingConstructor]
        public SaveActiveFileCommandDefinition(IDockingHostViewModel dockingHostViewModel)
        {
            dockingHostViewModel.ActiveLayoutItemChanged += _dockingHostViewModel_ActiveLayoutItemChanged;
        }

        private void _dockingHostViewModel_ActiveLayoutItemChanged(object sender, LayoutChangeEventArgs e)
        {
            _text = e.NewLayoutItem?.DisplayName;
            OnPropertyChanged(nameof(Text));
        }
    }
}
