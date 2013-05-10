using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressivityEngine;


namespace ExpressYourself
{
    class _Main
    {
        static void Main(string[] args)
        {
            var app = new Application(new ShutdownInterceptor());
        }
    }
}
