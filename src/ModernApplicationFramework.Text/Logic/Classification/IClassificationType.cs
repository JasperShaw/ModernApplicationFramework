using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public interface IClassificationType
    {
        IEnumerable<IClassificationType> BaseTypes { get; }
        string Classification { get; }

        bool IsOfType(string type);
    }
}