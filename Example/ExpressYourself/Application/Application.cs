using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Networking;
using ExpressivityEngine;

namespace ExpressYourself
{
    class Application : ILoggingHandler, IExpressivity
    {
        private readonly Server _server = null;
        private readonly bool _running;

        private readonly SkeletonDispatcher _skeletonDispatcher;
        private readonly KinectManager _kinectManager;
        private readonly ExpressivityCalculator _expressivityCalculator;


        public Application(ShutdownInterceptor shutdownInterceptor)
        {
            _running = true;

            _skeletonDispatcher = new SkeletonDispatcher();
            _kinectManager = new KinectManager(this, _skeletonDispatcher);
            _expressivityCalculator = new ExpressivityCalculator(_skeletonDispatcher, _kinectManager, this, this);

            shutdownInterceptor.registerShutdownHandler(handleShutdownIntercept);

            _server = new Server(Properties.Resources.NetworkPort);
        }

        private void handleShutdownIntercept()
        {
            _server.Stop();
            _kinectManager.Shutdown();
        }

        private void quit()
        {
            _server.Stop();
            _kinectManager.Shutdown();
        }


        // ILogger interface
        public void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }

        // IExpressivity interface
        public void ExpressivityUpdate(double X, double Y, double Z, double magnitude)
        {
            _server.Send( VectorStringBuilder.BuildString(X, Y, Z, magnitude) );
        }
    }
}
