using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Tool
{
    public interface IOutputView
    {
        void Clear();
        void ScrollToEnd();
        void AppendText(string text);
        void SetText(string text);
    }
}
