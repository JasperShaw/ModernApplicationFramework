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
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(DeleteCommandDefinition))]
    public class DeleteCommandDefinition : CommandDefinition<IDeleteCommand>
    {
        public override string NameUnlocalized => Commands_Resources.ResourceManager.GetString(nameof(Commands_Resources.DeleteCommandDefinition_Text),
            CultureInfo.InvariantCulture);
        public override string Text => Commands_Resources.DeleteCommandDefinition_Text;
        public override string ToolTip => Text;

        public override ImageMoniker ImageMonikerSource => Monikers.Delete;
        public override CommandBarCategory Category => CommandCategories.EditCategory;
        public override Guid Id => new Guid("{667CA2DA-8DBD-4D93-8167-007A38A82A2B}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.Delete))
        });
    }
}
