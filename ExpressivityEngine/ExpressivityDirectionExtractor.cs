using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;


namespace ExpressivityEngine
{
    public class ExpressivityDirectionExtractor
    {
        private readonly string _name;
        private readonly JointType[] _chain;
        private readonly Vector3D _neutral;
        private double _maxDistance;

        public ExpressivityDirectionExtractor(string name, JointType[] chain, Vector3D neutral)
        {
            _maxDistance = 0;

            _name = name;

            _chain = chain;

            _neutral = neutral;
            _neutral.Normalize();
        }

        public Vector3D Extract(Skeleton skeleton)
        {
            Vector3D result;
            int origin_idx = 0;
            int endpoint_idx = _chain.Length - 1;

            SkeletonPoint origin = skeleton.Joints[_chain[origin_idx]].Position;
            SkeletonPoint endpoint = skeleton.Joints[_chain[endpoint_idx]].Position;

            result = Utilities.CalculateVector(origin, endpoint);

            return Vector3D.Subtract(result, CalculateNeutralVector(skeleton));
        }

        private Vector3D CalculateNeutralVector(Skeleton skeleton)
        {
            double distance = 0;

            for (int i = 1; i < _chain.Length; i++)
            {
                SkeletonPoint A, B;

                A = skeleton.Joints[_chain[i - 1]].Position;
                B = skeleton.Joints[_chain[i]].Position;

                distance += Math.Sqrt(
                    Math.Pow(A.X - B.X, 2) +
                    Math.Pow(A.Y - B.Y, 2) +
                    Math.Pow(A.Z - B.Z, 2));
            }

            _maxDistance = Math.Max(_maxDistance, distance);

            return Vector3D.Multiply(_neutral, _maxDistance);
        }
    }
}
