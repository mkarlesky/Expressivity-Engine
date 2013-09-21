using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;


namespace ExpressivityEngine
{
    public class ExpressivityMagnitudeExtractor
    {
        public class KineticSegment
        {
            private readonly string _name;
            private readonly JointType _proximal;
            private readonly JointType _distal;
            private readonly double _mass;
            private readonly double _centerOfMassFactor;

            public double Mass { get { return _mass; } }
            public string Name { get { return _name; } }
            public JointType ReferenceJoint { get { return _proximal; } }
            public ExpressivityMagnitudeExtractor Extractor { get; set; }

            public KineticSegment(
                string name,
                JointType proximal,
                JointType distal,
                double mass,
                double centerOfMassFactor)
            {
                _name     = name;
                _proximal = proximal;
                _distal   = distal;
                _mass     = mass;
                _centerOfMassFactor = centerOfMassFactor;
            }

            public Point3D CalculateCenterOfMass(Skeleton skeleton)
            {
                Vector3D vector =
                    Utilities.CalculateVector(
                        skeleton.Joints[_proximal].Position,
                        skeleton.Joints[_distal].Position);
                vector = Vector3D.Multiply(vector, _centerOfMassFactor);

                return Point3D.Add(
                    Utilities.CalculatePoint(skeleton.Joints[_proximal].Position),
                    vector);
            }
        }

        private static Vector3D VERTICAL_VECTOR = new Vector3D(0.0, -1.0, 0.0);
        private readonly KineticSegment[] _chain;
        private readonly double _scalingFactor;

        public ExpressivityMagnitudeExtractor(string name, KineticSegment[] chain, double scalingFactor=1.0)
        {
            Name = name;

            _chain         = chain;
            _scalingFactor = scalingFactor;

            foreach (KineticSegment segment in chain)
            {
                segment.Extractor = this;
            }
        }

        public ExpressivityMagnitudeExtractor(ExpressivityMagnitudeExtractor extractor, double scalingFactor) :
            this(extractor.Name, extractor._chain, scalingFactor)
        {
        }

        public string Name{ get; private set; }

        public double Extract(Skeleton skeleton)
        {
            int i, j;
            double torque = 0;

            for (i = 0; i < _chain.Length; i++)
            {
                KineticSegment[] chain = new KineticSegment[_chain.Length - i];

                for (j = 0; j < chain.Length; j++)
                {
                    chain[j] = _chain[i+j];
                }

                torque += Extract(skeleton, chain, _chain[0].ReferenceJoint);
            }
            return torque;
        }

        private double Extract(Skeleton skeleton, KineticSegment[] chain, JointType coordinateReference)
        {
            double totalMass = 0;
            Point3D centerOfMass = new Point3D();
            Point3D reference = Utilities.CalculatePoint(skeleton.Joints[coordinateReference].Position);

            foreach (KineticSegment segment in chain)
            {
                totalMass += segment.Mass;
            }

            foreach (KineticSegment segment in chain)
            {
                Point3D point = segment.CalculateCenterOfMass(skeleton);

                centerOfMass.X += point.X * segment.Mass;
                centerOfMass.Y += point.Y * segment.Mass;
                centerOfMass.Z += point.Z * segment.Mass;
            }

            centerOfMass.X /= totalMass;
            centerOfMass.Y /= totalMass;
            centerOfMass.Z /= totalMass;

            return CalculateTorque(reference, centerOfMass, totalMass);
        }

        private double CalculateTorque(Point3D reference, Point3D centerOfMass, double totalMass)
        {
            Vector3D vector = Utilities.CalculateVector(reference, centerOfMass);

            double angle = Vector3D.AngleBetween(VERTICAL_VECTOR, vector);

            double perpindicularDistance = Math.Sin(Utilities.DegreesToRadians(angle)) * vector.Length;
            return perpindicularDistance * totalMass * _scalingFactor;
        }
    }
}
