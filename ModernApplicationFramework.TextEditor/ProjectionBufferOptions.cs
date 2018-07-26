using System;

namespace ModernApplicationFramework.TextEditor
{
    [Flags]
    public enum ProjectionBufferOptions
    {
        None = 0,
        PermissiveEdgeInclusiveSourceSpans = 1,
        WritableLiteralSpans = 2,
    }
}