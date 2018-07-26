using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
{
    public interface IViewTaggerMetadata : INamedTaggerMetadata
    {
        [DefaultValue(null)]
        IEnumerable<string> TextViewRoles { get; }
    }
}