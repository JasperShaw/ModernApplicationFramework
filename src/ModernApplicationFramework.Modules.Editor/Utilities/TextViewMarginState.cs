using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class TextViewMarginState : IPartImportsSatisfiedNotification
    {
        internal ImmutableDictionary<string, List<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>>> MarginMap = ImmutableDictionary<string, List<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>>>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);

        [ImportMany]
        internal List<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>> MarginProviders { get; set; }

        public IList<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>> OrderedMarginProviders { get; set; }

        public void OnImportsSatisfied()
        {
            OrderedMarginProviders = Orderer.Order(MarginProviders);
        }

        public IReadOnlyList<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>> GetMarginProviders(string containerName)
        {
            var marginProviders = new List<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>>();
            if (!MarginMap.TryGetValue(containerName, out marginProviders))
            {
                marginProviders = OrderedMarginProviders.Where(orderedMarginProvider => StringComparer.OrdinalIgnoreCase.Equals(orderedMarginProvider.Metadata.MarginContainer, containerName)).ToList();
                ImmutableInterlocked.Update(ref MarginMap, s => s.Add(containerName, marginProviders));
            }
            return marginProviders;
        }
    }
}