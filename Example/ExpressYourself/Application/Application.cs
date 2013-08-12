using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Networking;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;
using ExpressivityEngine;

namespace ExpressYourself
{
    class Application : ILoggingHandler
    {
        private readonly Server _server = null;

        private readonly SkeletonDispatcher _skeletonDispatcher;
        private readonly KinectManager _kinectManager;
        private readonly ExpressivityCalculator _expressivityCalculator;

        public Application(ShutdownInterceptor shutdownInterceptor)
        {
            _skeletonDispatcher = new SkeletonDispatcher();
            _skeletonDispatcher.RegisterSkeletonHandler(SkeletonHandler);

            _kinectManager = new KinectManager(this, _skeletonDispatcher);

            _expressivityCalculator = new ExpressivityCalculator();

            shutdownInterceptor.registerShutdownHandler(handleShutdownIntercept);

            _server = new Server(Properties.Resources.NetworkPort);
        }

        public void Setup()
        {
            List<ExpressivityMagnitudeExtractor> _magnitudeElements = new List<ExpressivityMagnitudeExtractor>();
            List<ExpressivityDirectionExtractor> _directionElements = new List<ExpressivityDirectionExtractor>();

            _magnitudeElements.Add(ExpressivityCalculator.MAGNITUDE_EXTRACTOR_RIGHT_ARM);
            _magnitudeElements.Add(ExpressivityCalculator.MAGNITUDE_EXTRACTOR_LEFT_ARM);
            _magnitudeElements.Add(ExpressivityCalculator.MAGNITUDE_EXTRACTOR_TRUNK);

            _directionElements.Add(ExpressivityCalculator.DIRECTION_EXTRACTOR_LEFT_ARM);
            _directionElements.Add(ExpressivityCalculator.DIRECTION_EXTRACTOR_RIGHT_ARM);
            _directionElements.Add(ExpressivityCalculator.DIRECTION_EXTRACTOR_RIGHT_SHOULDER);
            _directionElements.Add(ExpressivityCalculator.DIRECTION_EXTRACTOR_LEFT_SHOULDER);
            _directionElements.Add(ExpressivityCalculator.DIRECTION_EXTRACTOR_TRUNK);

            _expressivityCalculator.SetMagnitudeExtractors(_magnitudeElements);
            _expressivityCalculator.SetDirectionExtractors(_directionElements);
        }

        // ILogger interface
        public void WriteMessage(string message)
        {
            Console.WriteLine(message);
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

        private void SkeletonHandler(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;

            foreach (Skeleton s in skeletons)
            {
                if (s.TrackingState == SkeletonTrackingState.Tracked)
                {
                    skeleton = s;
                    break;
                }
            }

            if (skeleton == null)
                return;

            // Flush data structure if we get any skeletons of not fully valid joint data
            if (!SkeletonDispatcher.IsSkeletonValid(skeleton, _kinectManager.SkeletonTrackingMode))
            {
                return;
            }

            Vector3D result = _expressivityCalculator.CalculateDirection(skeleton);
            double magnitude = _expressivityCalculator.CalculateMagnitude(skeleton);

            ExpressivityUpdate( result.X, result.Y, result.Z, magnitude );
        }

        private void ExpressivityUpdate(double X, double Y, double Z, double magnitude)
        {
            _server.Send( VectorStringBuilder.BuildString(X, Y, Z, magnitude) );
        }
    }
}
