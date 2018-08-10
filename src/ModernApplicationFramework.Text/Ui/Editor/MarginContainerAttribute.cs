using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class MarginContainerAttribute : SingletonBaseMetadataAttribute
    {
        public MarginContainerAttribute(string marginContainer)
        {
            if (marginContainer == null)
                throw new ArgumentNullException(nameof(marginContainer));
            if (marginContainer.Length == 0)
                throw new ArgumentException("marginContainer is an empty string.");
            MarginContainer = marginContainer;
        }

        public string MarginContainer { get; }
    }
}