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
    [Export(typeof(CloseProgramCommandDefinition))]
    public sealed class CloseProgramCommandDefinition : CommandDefinition<ICloseProgramCommand>
    {
        public override ImageMoniker ImageMonikerSource => Monikers.CloseProgram;

        public override string Text => Commands_Resources.CloseProgramCommandDefinition_Text;

        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("CloseProgramCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string ToolTip => null;

        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{78CD7FA8-147F-4464-814B-DB36438145CB}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.F4, ModifierKeys.Alt))
        });
    }
}