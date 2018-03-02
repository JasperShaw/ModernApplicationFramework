using System.Collections.Generic;
using System.Linq;

namespace ModernApplicationFramework.EditorBase.Core.OpenSaveDialogFilters
{
    public class FilterDataEntry
    {
        public string Text { get; }

        public IEnumerable<string> Extensions { get; }

        public FilterDataEntry(string text, IEnumerable<string> extensions)
        {
            Text = text;
            Extensions = extensions;
        }

        public FilterDataEntry(string text, string extension) : this(text, new List<string> { extension })
        {
        }

        public override string ToString()
        {
            return !Extensions.Any()
                ? string.Empty
                : $"{Text} ({BuildExtensionsList(',')})|{BuildExtensionsList(';')}";
        }

        private string BuildExtensionsList(char separationChar)
        {
            var list = Extensions.Aggregate(string.Empty,
                (current, extension) => current + $"*{extension}{separationChar} ");

            list = list.Remove(list.Length - 2, 2);

            return list;
        }
    }
}