using System.IO;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Modules.OutputTool
{
    public interface IOutput : ITool
    {
        TextWriter Writer { get; }
        void AppendLine(string text);
        void Append(string text);
        void Clear();
    }
}
