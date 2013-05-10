using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressivityEngine
{
    public interface ILoggingHandler
    {
        void WriteMessage(string message);
    }
}
