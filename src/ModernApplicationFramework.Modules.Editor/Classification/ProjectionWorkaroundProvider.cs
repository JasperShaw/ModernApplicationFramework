using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Classification
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("projection")]
    [TagType(typeof(ClassificationTag))]
    internal class ProjectionWorkaroundProvider : ITaggerProvider
    {
        [Import] internal IDifferenceService DiffService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var projectionBuffer = buffer as IProjectionBuffer;
            if (projectionBuffer == null)
                return null;
            return new ProjectionWorkaroundTagger(projectionBuffer, DiffService) as ITagger<T>;
        }
    }
}