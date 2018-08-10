using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class GridCellLengthAttribute : SingletonBaseMetadataAttribute
    {
        public double GridCellLength { get; }

        public GridCellLengthAttribute(double cellLength)
        {
            GridCellLength = cellLength;
        }
    }
}