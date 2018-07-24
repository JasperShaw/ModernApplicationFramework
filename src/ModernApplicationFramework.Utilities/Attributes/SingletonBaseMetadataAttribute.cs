using System;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Utilities.Attributes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class SingletonBaseMetadataAttribute : Attribute
    {
    }
}
