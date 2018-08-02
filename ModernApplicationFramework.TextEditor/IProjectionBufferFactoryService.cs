using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface IProjectionBufferFactoryService
    {
        IContentType ProjectionContentType { get; }

        IProjectionBuffer CreateProjectionBuffer(IProjectionEditResolver projectionEditResolver, IList<object> sourceSpans, ProjectionBufferOptions options, IContentType contentType);

        IProjectionBuffer CreateProjectionBuffer(IProjectionEditResolver projectionEditResolver, IList<object> sourceSpans, ProjectionBufferOptions options);

        IElisionBuffer CreateElisionBuffer(IProjectionEditResolver projectionEditResolver, NormalizedSnapshotSpanCollection exposedSpans, ElisionBufferOptions options, IContentType contentType);

        IElisionBuffer CreateElisionBuffer(IProjectionEditResolver projectionEditResolver, NormalizedSnapshotSpanCollection exposedSpans, ElisionBufferOptions options);

        event EventHandler<TextBufferCreatedEventArgs> ProjectionBufferCreated;
    }
}