using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public interface IClassificationType
    {
        string Classification { get; }

        bool IsOfType(string type);

        IEnumerable<IClassificationType> BaseTypes { get; }
    }
}