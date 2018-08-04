using System;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Utilities.Attributes
{
    public class ImportImplementationsAttribute : ImportManyAttribute
    {
        public ImportImplementationsAttribute(Type contractType)
            : base("ModernApplicationFramework.Utilities.Export.Implementation", contractType)
        {
        }
    }
}
