using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.CommandBar.Resources;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(RedoCommandDefinition))]
    public sealed class RedoCommandDefinition : CommandDefinition<IRedoCommand>
    {
        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.Y, ModifierKeys.Control))
        });

        public override ImageMoniker ImageMonikerSource => Monikers.Redo;

        public override string Text => Commands_Resources.RedoCommandDefinition_Text;
        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("RedoCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Text;

        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{6B8097A9-50BE-4966-83ED-CC3EA25CF5B7}");
    }
}