using System;
using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class GridUnitTypeAttribute : SingletonBaseMetadataAttribute
    {
        public GridUnitType GridUnitType { get; }

        public GridUnitTypeAttribute(GridUnitType gridUnitType)
        {
            GridUnitType = gridUnitType;
        }
    }
}