using System;

namespace ModernApplicationFramework.Text.Data.Projection
{
    [Flags]
    public enum ProjectionBufferOptions
    {
        None = 0,
        PermissiveEdgeInclusiveSourceSpans = 1,
        WritableLiteralSpans = 2
    }
}