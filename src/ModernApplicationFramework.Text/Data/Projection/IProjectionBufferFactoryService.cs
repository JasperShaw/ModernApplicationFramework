using System;
using System.Collections.Generic;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public interface IProjectionBufferFactoryService
    {
        event EventHandler<TextBufferCreatedEventArgs> ProjectionBufferCreated;
        IContentType ProjectionContentType { get; }

        IElisionBuffer CreateElisionBuffer(IProjectionEditResolver projectionEditResolver,
            NormalizedSnapshotSpanCollection exposedSpans, ElisionBufferOptions options, IContentType contentType);

        IElisionBuffer CreateElisionBuffer(IProjectionEditResolver projectionEditResolver,
            NormalizedSnapshotSpanCollection exposedSpans, ElisionBufferOptions options);

        IProjectionBuffer CreateProjectionBuffer(IProjectionEditResolver projectionEditResolver,
            IList<object> sourceSpans, ProjectionBufferOptions options, IContentType contentType);

        IProjectionBuffer CreateProjectionBuffer(IProjectionEditResolver projectionEditResolver,
            IList<object> sourceSpans, ProjectionBufferOptions options);
    }
}