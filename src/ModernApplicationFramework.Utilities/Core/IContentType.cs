using System.Collections.Generic;

namespace ModernApplicationFramework.Utilities.Core
{
    public interface IContentType
    {
        string TypeName { get; }

        string DisplayName { get; }

        bool IsOfType(string type);

        IEnumerable<IContentType> BaseTypes { get; }
    }
}
