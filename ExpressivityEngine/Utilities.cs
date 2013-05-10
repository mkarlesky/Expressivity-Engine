using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;


namespace ExpressivityEngine
{
    class Utilities
    {
        public static Vector3D CalculateVector(SkeletonPoint origin, SkeletonPoint endpoint)
        {
            return new Vector3D(
                endpoint.X - origin.X,
                endpoint.Y - origin.Y,
                origin.Z   - endpoint.Z );
        }

        public static Vector3D CalculateVector(Point3D origin, Point3D endpoint)
        {
            return new Vector3D(
                endpoint.X - origin.X,
                endpoint.Y - origin.Y,
                endpoint.Z - origin.Z);
        }

        public static Point3D CalculatePoint(SkeletonPoint point)
        {
            return new Point3D(
                 point.X,
                 point.Y,
                -point.Z );
        }

        public static Point3D SubtractPoints(SkeletonPoint A, SkeletonPoint B)
        {
            return new Point3D(
                A.X - B.X,
                A.Y - B.Y,
                B.Z - A.Z );
        }

        public static double DegreesToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
