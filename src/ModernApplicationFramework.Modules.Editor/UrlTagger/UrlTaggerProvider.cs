using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.UrlTagger
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(IUrlTag))]
    [Name("UrlTagger")]
    internal sealed class UrlTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IEditorOptionsFactoryService EditorOptionsFactory;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new UrlTagger(EditorOptionsFactory.GetOptions(buffer).GetOptionValue(DefaultOptions.LongBufferLineThresholdId)) as ITagger<T>;
        }
    }
}
