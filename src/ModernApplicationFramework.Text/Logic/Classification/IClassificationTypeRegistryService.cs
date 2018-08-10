using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public interface IClassificationTypeRegistryService
    {
        IClassificationType CreateClassificationType(string type, IEnumerable<IClassificationType> baseTypes);

        IClassificationType CreateTransientClassificationType(IEnumerable<IClassificationType> baseTypes);

        IClassificationType CreateTransientClassificationType(params IClassificationType[] baseTypes);
        IClassificationType GetClassificationType(string type);
    }
}