using System.ComponentModel;
using ModernApplicationFramework.Utilities.Core;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    public interface IAdornmentLayersMetadata : IOrderable
    {
        [DefaultValue(false)] bool IsOverlayLayer { get; }
    }
}