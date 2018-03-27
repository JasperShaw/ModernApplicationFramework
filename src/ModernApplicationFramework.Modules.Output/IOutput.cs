using System.IO;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Modules.Output
{
    public interface IOutput : ITool
    {
        TextWriter Writer { get; }
        void AppendLine(string text);
        void Append(string text);
        void Clear();
    }
}