using System.Collections.Generic;

namespace ModernApplicationFramework.Interfaces.Settings
{
    public interface ISettingsCategory
    {
        IList<ISettingsCategory> Children { get; }
        bool IsToolsOptionsCategory { get; }
        string Name { get; }
        ISettingsCategory Parent { get; }
        IEnumerable<ISettingsCategory> Path { get; }
        ISettingsCategory Root { get; }
        uint SortOrder { get; }
        string Text { get; }
    }
}
