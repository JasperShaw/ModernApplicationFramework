using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface IClassificationType
    {
        string Classification { get; }

        bool IsOfType(string type);

        IEnumerable<IClassificationType> BaseTypes { get; }
    }
}