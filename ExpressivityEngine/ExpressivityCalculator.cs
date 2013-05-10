using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;


namespace ExpressivityEngine
{
    public class ExpressivityCalculator
    {
        private static readonly ExpressivityDirectionExtractor[] _directionExtractorList = {
            new ExpressivityDirectionExtractor(
                "Right arm",
                new JointType[]{JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight},
                new Vector3D(0.2, -0.9, 0)),
            new ExpressivityDirectionExtractor(
                "Left arm",
                new JointType[]{JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft},
                new Vector3D(-0.2, -0.9, 0)),
            new ExpressivityDirectionExtractor(
                "Trunk",
                new JointType[]{JointType.Spine, JointType.ShoulderCenter, JointType.Head},
                new Vector3D(0, 1, 0)),
            new ExpressivityDirectionExtractor(
                "Right shoulder / clavicle",
                new JointType[]{JointType.ShoulderCenter, JointType.ShoulderRight},
                new Vector3D(0.85, -0.55, 0)),
            new ExpressivityDirectionExtractor(
                "Left shoulder / clavicle",
                new JointType[]{JointType.ShoulderCenter, JointType.ShoulderLeft},
                new Vector3D(-0.85, -0.55, 0)),
        };

        private static readonly ExpressivityMagnitudeExtractor[] _magnitudeExtractorList = {
            new ExpressivityMagnitudeExtractor(
                "Right arm",
                new ExpressivityMagnitudeExtractor.KineticSegment[]{
                    new ExpressivityMagnitudeExtractor.KineticSegment("Upper arm", JointType.ShoulderRight, JointType.ElbowRight, 0.028, 0.436),
                    new ExpressivityMagnitudeExtractor.KineticSegment("Forearm", JointType.ElbowRight, JointType.WristRight, 0.016, 0.430),
                    // Center of mass factor for hand is 1.0 because hand joint is center of hand
                    new ExpressivityMagnitudeExtractor.KineticSegment("Hand", JointType.WristRight, JointType.HandRight, 0.006, 1.0)}
                    ),
            new ExpressivityMagnitudeExtractor(
                "Left arm",
                new ExpressivityMagnitudeExtractor.KineticSegment[]{
                    new ExpressivityMagnitudeExtractor.KineticSegment("Upper arm", JointType.ShoulderLeft, JointType.ElbowLeft, 0.028, 0.436),
                    new ExpressivityMagnitudeExtractor.KineticSegment("Forearm", JointType.ElbowLeft, JointType.WristLeft, 0.016, 0.430),
                    // Center of mass factor for hand is 1.0 because hand joint is center of hand
                    new ExpressivityMagnitudeExtractor.KineticSegment("Hand", JointType.WristLeft, JointType.HandLeft, 0.006, 1.0)}
                    ),
            new ExpressivityMagnitudeExtractor(
                "Torso / head",
                new ExpressivityMagnitudeExtractor.KineticSegment[]{
//                    new ExpressivityMagnitudeExtractor.KineticSegment("Torso", JointType.HipCenter, JointType.ShoulderCenter, 0.355, 0.63),
                    // Center of mass factor for head is 1.0 because head joint is center of head
                    new ExpressivityMagnitudeExtractor.KineticSegment("Head + neck", JointType.ShoulderCenter, JointType.Head, 0.081, 1.0)}
                    ),
        };

        private readonly ILoggingHandler _logger;
        private readonly IExpressivity _expressivity;
        private readonly KinectManager _kinectManager;

        public ExpressivityCalculator(SkeletonDispatcher skeletonDispatcher, KinectManager kinectManager, ILoggingHandler logger, IExpressivity expressivity)
        {
            skeletonDispatcher.RegisterSkeletonHandler(SkeletonHandler);
            _logger = logger;
            _expressivity = expressivity;
            _kinectManager = kinectManager;
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

            Vector3D result = CalculateVectors(skeleton);

            _expressivity.ExpressivityUpdate(result.X, result.Y, result.Z, CalculateTorques(skeleton));
        }

        private Vector3D CalculateVectors(Skeleton skeleton)
        {
            Vector3D result = new Vector3D(0, 0, 0);

            foreach (ExpressivityDirectionExtractor extractor in _directionExtractorList)
            {
                result = Vector3D.Add(result, extractor.Extract(skeleton));
            }

            result.Normalize();
            return result;
        }

        private double CalculateTorques(Skeleton skeleton)
        {
            double torqueTotal = 0;

            foreach (ExpressivityMagnitudeExtractor extractor in _magnitudeExtractorList)
            {
                torqueTotal += extractor.Extract(skeleton);
            }

            return torqueTotal;
        }
    }
}
