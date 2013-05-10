using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ExpressYourself
{
    public class ShutdownInterceptor
    {
        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        public delegate void ShutdownInterceptDelegate();
        private ShutdownInterceptDelegate _shutdownInterceptDelegates = null;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private delegate bool EventHandler(CtrlType sig);

        private static EventHandler _ShutdownInteceptHandler;

        public ShutdownInterceptor()
        {
            _ShutdownInteceptHandler += new EventHandler(ShutdownInterceptHandler);
            SetConsoleCtrlHandler(_ShutdownInteceptHandler, true);
        }

        public void registerShutdownHandler(ShutdownInterceptDelegate handler)
        {
            _shutdownInterceptDelegates += handler;
        }

        private bool ShutdownInterceptHandler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    _shutdownInterceptDelegates();
                    break;
            }

            // Tell system we'll handle shutdown, thanks
            // Note: system only gives maybe 2 or 3 seconds to actually shutdown
            return true;
        }
    }
}
