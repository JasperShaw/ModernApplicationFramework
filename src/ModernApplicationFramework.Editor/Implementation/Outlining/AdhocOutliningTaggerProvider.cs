using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.Implementation.Outlining
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("any")]
    [TagType(typeof(IOutliningRegionTag))]
    internal class AdhocOutliningTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer is IElisionBuffer)
                return null;
            return GetOutliningTagger(buffer) as ITagger<T>;
        }

        public static SimpleTagger<IOutliningRegionTag> GetOutliningTagger(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(typeof(SimpleTagger<IOutliningRegionTag>), () => new SimpleTagger<IOutliningRegionTag>(buffer));
        }
    }
}
