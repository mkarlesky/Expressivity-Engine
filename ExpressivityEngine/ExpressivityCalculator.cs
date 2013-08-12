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
        public static readonly ExpressivityDirectionExtractor DIRECTION_EXTRACTOR_RIGHT_ARM =
            new ExpressivityDirectionExtractor(
                "Right arm",
                new JointType[]{JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight},
                new Vector3D(0.2, -0.9, 0));

        public static readonly ExpressivityDirectionExtractor DIRECTION_EXTRACTOR_LEFT_ARM =
            new ExpressivityDirectionExtractor(
                "Left arm",
                new JointType[]{JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft},
                new Vector3D(-0.2, -0.9, 0));

        public static readonly ExpressivityDirectionExtractor DIRECTION_EXTRACTOR_TRUNK =
            new ExpressivityDirectionExtractor(
                "Trunk",
                new JointType[]{JointType.Spine, JointType.ShoulderCenter, JointType.Head},
                new Vector3D(0, 1, 0));

        public static readonly ExpressivityDirectionExtractor DIRECTION_EXTRACTOR_RIGHT_SHOULDER =
            new ExpressivityDirectionExtractor(
                "Right shoulder", // Clavicle area
                new JointType[]{JointType.ShoulderCenter, JointType.ShoulderRight},
                new Vector3D(0.85, -0.55, 0));

        public static readonly ExpressivityDirectionExtractor DIRECTION_EXTRACTOR_LEFT_SHOULDER =
            new ExpressivityDirectionExtractor(
                "Left shoulder", // Clavicle area
                new JointType[]{JointType.ShoulderCenter, JointType.ShoulderLeft},
                new Vector3D(-0.85, -0.55, 0));

        public static readonly ExpressivityDirectionExtractor DIRECTION_EXTRACTOR_LEFT_LEG =
            new ExpressivityDirectionExtractor(
                "Left leg",
                new JointType[] { JointType.HipLeft, JointType.KneeLeft, JointType.AnkleLeft },
                new Vector3D(0, -1, 0));

        public static readonly ExpressivityDirectionExtractor DIRECTION_EXTRACTOR_RIGHT_LEG =
            new ExpressivityDirectionExtractor(
                "Right leg",
                new JointType[] { JointType.HipRight, JointType.KneeRight, JointType.AnkleRight },
                new Vector3D(0, -1, 0));



        public static readonly ExpressivityMagnitudeExtractor MAGNITUDE_EXTRACTOR_RIGHT_ARM = 
            new ExpressivityMagnitudeExtractor(
                "Right arm",
                new ExpressivityMagnitudeExtractor.KineticSegment[]{
                    new ExpressivityMagnitudeExtractor.KineticSegment("Upper arm", JointType.ShoulderRight, JointType.ElbowRight, 0.028, 0.436),
                    new ExpressivityMagnitudeExtractor.KineticSegment("Forearm", JointType.ElbowRight, JointType.WristRight, 0.016, 0.430),
                    // Center of mass factor for hand is 1.0 because hand joint is center of hand
                    new ExpressivityMagnitudeExtractor.KineticSegment("Hand", JointType.WristRight, JointType.HandRight, 0.006, 1.0)}
                    );

        public static readonly ExpressivityMagnitudeExtractor MAGNITUDE_EXTRACTOR_LEFT_ARM = 
            new ExpressivityMagnitudeExtractor(
                "Left arm",
                new ExpressivityMagnitudeExtractor.KineticSegment[]{
                    new ExpressivityMagnitudeExtractor.KineticSegment("Upper arm", JointType.ShoulderLeft, JointType.ElbowLeft, 0.028, 0.436),
                    new ExpressivityMagnitudeExtractor.KineticSegment("Forearm", JointType.ElbowLeft, JointType.WristLeft, 0.016, 0.430),
                    // Center of mass factor for hand is 1.0 because hand joint is center of hand
                    new ExpressivityMagnitudeExtractor.KineticSegment("Hand", JointType.WristLeft, JointType.HandLeft, 0.006, 1.0)}
                    );

        public static readonly ExpressivityMagnitudeExtractor MAGNITUDE_EXTRACTOR_TRUNK = 
            new ExpressivityMagnitudeExtractor(
                "Trunk",
                new ExpressivityMagnitudeExtractor.KineticSegment[]{
                    new ExpressivityMagnitudeExtractor.KineticSegment("Torso", JointType.HipCenter, JointType.ShoulderCenter, 0.355, 0.63),
                    // Center of mass factor for head is 1.0 because head joint is center of head
                    new ExpressivityMagnitudeExtractor.KineticSegment("Head + neck", JointType.ShoulderCenter, JointType.Head, 0.081, 1.0)}
                    );

        public static readonly ExpressivityMagnitudeExtractor MAGNITUDE_EXTRACTOR_RIGHT_LEG =
            new ExpressivityMagnitudeExtractor(
                "Right leg",
                new ExpressivityMagnitudeExtractor.KineticSegment[]{
                    new ExpressivityMagnitudeExtractor.KineticSegment("Thigh", JointType.HipRight, JointType.KneeRight, 0.1, 0.433),
                    new ExpressivityMagnitudeExtractor.KineticSegment("Lower leg", JointType.KneeRight, JointType.AnkleRight, 0.0465, 0.433),
                    // Center of mass factor for foot is 1.0 because foot joint is center of foot
                    new ExpressivityMagnitudeExtractor.KineticSegment("Foot", JointType.AnkleRight, JointType.FootRight, 0.0145, 1.0)}
                    );

        public static readonly ExpressivityMagnitudeExtractor MAGNITUDE_EXTRACTOR_LEFT_LEG =
            new ExpressivityMagnitudeExtractor(
                "Left leg",
                new ExpressivityMagnitudeExtractor.KineticSegment[]{
                    new ExpressivityMagnitudeExtractor.KineticSegment("Thigh", JointType.HipLeft, JointType.KneeLeft, 0.1, 0.433),
                    new ExpressivityMagnitudeExtractor.KineticSegment("Lower leg", JointType.KneeLeft, JointType.AnkleLeft, 0.0465, 0.433),
                    // Center of mass factor for foot is 1.0 because foot joint is center of foot
                    new ExpressivityMagnitudeExtractor.KineticSegment("Foot", JointType.AnkleLeft, JointType.FootLeft, 0.0145, 1.0)}
                    );


        private List<ExpressivityMagnitudeExtractor> _magnitudeExtractorList;
        private List<ExpressivityDirectionExtractor> _directionExtractorList;

        public ExpressivityCalculator()
        {
            _magnitudeExtractorList = new List<ExpressivityMagnitudeExtractor>();
            _directionExtractorList = new List<ExpressivityDirectionExtractor>();
        }

        public void SetMagnitudeExtractors(List<ExpressivityMagnitudeExtractor> list)
        {
            _magnitudeExtractorList = list;
        }

        public void SetDirectionExtractors(List<ExpressivityDirectionExtractor> list)
        {
            _directionExtractorList = list;
        }

        public Vector3D CalculateDirection(Skeleton skeleton)
        {
            return CalculateVectors(skeleton);
        }

        public double CalculateMagnitude(Skeleton skeleton)
        {
            return CalculateTorques(skeleton);
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
