using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Kinect;

namespace ExpressYourself
{
    public class KinectManager
    {
        public static JointType[] SeatedJointList = {
            JointType.Head,
            JointType.ShoulderLeft,
            JointType.ShoulderCenter,
            JointType.ShoulderRight,
            JointType.ElbowLeft,
            JointType.ElbowRight,
            JointType.WristLeft,
            JointType.WristRight,
            JointType.HandLeft,
            JointType.HandRight};

        public static JointType[] DefaultPartialJointList = {
            JointType.Head,
            JointType.ShoulderLeft,
            JointType.ShoulderCenter,
            JointType.ShoulderRight,
            JointType.ElbowLeft,
            JointType.ElbowRight,
            JointType.WristLeft,
            JointType.WristRight,
            JointType.HandLeft,
            JointType.HandRight,
            JointType.Spine,
            JointType.HipCenter};

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor _sensor;
        private readonly ILoggingHandler _logger;
        private readonly SkeletonDispatcher _skeletonDispatcher;

        public KinectManager(ILoggingHandler logger, SkeletonDispatcher skeletonDispatcher)
        {
            _sensor = null;

            KinectSensor.KinectSensors.StatusChanged += KinectStatusChanged;

            _logger = logger;
            _skeletonDispatcher = skeletonDispatcher;

            _logger.WriteMessage(Properties.Resources.KinectWaiting);
        }

        public void Shutdown()
        {
            if (null != _sensor)
            {
                _sensor.Stop();
            }
        }

        public DepthImagePoint MapSkeletonPointToDepthPoint(SkeletonPoint point, DepthImageFormat format)
        {
            return _sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(point, format);
        }

        public SkeletonTrackingMode SkeletonTrackingMode
        {
            get { return _sensor.SkeletonStream.TrackingMode; }
        }

        private bool Initialize(KinectSensor sensor)
        {
            _sensor.DepthStream.Range = DepthRange.Near;
            _sensor.SkeletonStream.EnableTrackingInNearRange = true;
            //_sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;

            // Turn on the skeleton stream to receive skeleton frames
            _sensor.SkeletonStream.Enable();

            // Add an event handler to be called whenever there is new color frame data
            _sensor.SkeletonFrameReady += _skeletonDispatcher.SkeletonFrameReady;

            // Start the sensor!
            try
            {
                _sensor.Start();
            }
            catch (IOException)
            {
                _sensor = null;
            }

            return (null != _sensor);
        }


        private void KinectStatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    if (_sensor == null)
                    {
                        _sensor = e.Sensor;
                        if (Initialize(_sensor))
                            _logger.WriteMessage(Properties.Resources.KinectConnected);
                        else
                            _logger.WriteMessage(Properties.Resources.KinectUnavailable);
                    }
                    break;

                case KinectStatus.Disconnected:
                    if (_sensor == e.Sensor)
                    {
                        _sensor = null;
                        _logger.WriteMessage(Properties.Resources.KinectDisconnected);
                    }
                    break;

                case KinectStatus.NotReady:
                    if (_sensor == e.Sensor)
                    {
                        _sensor = null;
                        _logger.WriteMessage(Properties.Resources.KinectNotReady);
                    }
                    break;

                case KinectStatus.NotPowered:
                    if (_sensor == e.Sensor)
                    {
                        _sensor = null;
                        _logger.WriteMessage(Properties.Resources.KinectNoPower);
                    }
                    break;

                default:
                    // do nothing
                    break;
            }
        }
    }
}
