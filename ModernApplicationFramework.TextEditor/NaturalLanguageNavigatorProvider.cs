using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ITextStructureNavigatorProvider))]
    [ContentType("any")]
    internal sealed class NaturalLanguageNavigatorProvider : ITextStructureNavigatorProvider
    {
        public ITextStructureNavigator CreateTextStructureNavigator(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            return new NaturalLanguageNavigator(textBuffer);
        }
    }
}