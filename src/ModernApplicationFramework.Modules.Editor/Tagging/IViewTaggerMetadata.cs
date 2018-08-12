using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApplicationFramework.Modules.Editor.Tagging
{
    public interface IViewTaggerMetadata : INamedTaggerMetadata
    {
        [DefaultValue(null)]
        IEnumerable<string> TextViewRoles { get; }
    }
}