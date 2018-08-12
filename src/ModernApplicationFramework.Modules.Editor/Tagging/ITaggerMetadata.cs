using System;
using System.Collections.Generic;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Tagging
{
    public interface ITaggerMetadata : IContentTypeMetadata
    {
        IEnumerable<Type> TagTypes { get; }
    }
}