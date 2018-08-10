using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITaggerMetadata : IContentTypeMetadata
    {
        IEnumerable<Type> TagTypes { get; }
    }
}