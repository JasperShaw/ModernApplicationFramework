using System.ComponentModel;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.TextEditor
{
    public interface IAdornmentLayersMetadata : IOrderable
    {
        [DefaultValue(false)]
        bool IsOverlayLayer { get; }
    }
}