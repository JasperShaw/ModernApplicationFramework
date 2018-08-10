using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Logic.Editor
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class DeferCreationAttribute : SingletonBaseMetadataAttribute
    {
        private string _optionName = string.Empty;

        public string OptionName
        {
            get => _optionName;
            set => _optionName = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}