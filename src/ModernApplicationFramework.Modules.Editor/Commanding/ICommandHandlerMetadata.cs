using System.Collections.Generic;
using System.ComponentModel;
using ModernApplicationFramework.Utilities.Core;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Modules.Editor.Commanding
{
    public interface ICommandHandlerMetadata : IOrderable, IContentTypeMetadata
    {
        [DefaultValue(null)] IEnumerable<string> TextViewRoles { get; }
    }
}