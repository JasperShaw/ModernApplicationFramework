using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Logic.Classification
{
    public interface IClassificationTypeRegistryService
    {
        IClassificationType GetClassificationType(string type);

        IClassificationType CreateClassificationType(string type, IEnumerable<IClassificationType> baseTypes);

        IClassificationType CreateTransientClassificationType(IEnumerable<IClassificationType> baseTypes);

        IClassificationType CreateTransientClassificationType(params IClassificationType[] baseTypes);
    }
}