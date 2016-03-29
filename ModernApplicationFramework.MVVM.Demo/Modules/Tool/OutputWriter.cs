using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Tool
{
    internal class OutputWriter : TextWriter
    {
        private readonly IOutput _output;

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }

        public OutputWriter(IOutput output)
        {
            _output = output;
        }

        public override void WriteLine()
        {
            _output.AppendLine(string.Empty);
        }

        public override void WriteLine(string value)
        {
            _output.AppendLine(value);
        }

        public override void Write(string value)
        {
            _output.Append(value);
        }
    }
}
