using System.Collections.Generic;
using System.ComponentModel;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface ICommandHandlerMetadata : IOrderable, IContentTypeMetadata
    {
        [DefaultValue(null)] IEnumerable<string> TextViewRoles { get; }
    }
}