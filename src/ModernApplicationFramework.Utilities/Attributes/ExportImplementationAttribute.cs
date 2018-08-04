using System;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExportImplementationAttribute : ExportAttribute
    {
        internal const string ImplementationContractName = "ModernApplicationFramework.Utilities.Export.Implementation";

        public ExportImplementationAttribute(Type contractType)
            : base("ModernApplicationFramework.Utilities.Export.Implementation", contractType)
        {
        }
    }
}
