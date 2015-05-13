using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Core.Exception
{
    internal class ToolBarAlreadyExistsException : System.Exception
    {
        public ToolBarAlreadyExistsException()
        {

        }

        public ToolBarAlreadyExistsException(string message) : base(message)
        {

        }

        public ToolBarAlreadyExistsException(string message, System.Exception inner)
            : base(message, inner)
        {

        }
    }
}
