using System.Collections.Generic;
using System.Linq;

namespace ModernApplicationFramework.EditorBase.Core.OpenSaveDialogFilters
{
    public class FilterDataEntry
    {
        public string Text { get; }

        public ICollection<string> Extensions { get; }

        public FilterDataEntry(string text, IReadOnlyCollection<string> extensions)
        {
            Text = text;
            Extensions = new List<string>();
            foreach (var extension in extensions)
            {
                Extensions.Add(extension[0] != '.' ? $".{extension}" : extension);
            }
        }

        public FilterDataEntry(string text, string extension) : this(text, new List<string> { extension })
        {
        }

        public bool IsAnyFilter()
        {
            return Extensions.Count == 1 && Extensions.ElementAt(0).Equals(".*");
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