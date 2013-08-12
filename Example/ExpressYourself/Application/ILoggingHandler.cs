using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressYourself
{
    public interface ILoggingHandler
    {
        void WriteMessage(string message);
    }
}
