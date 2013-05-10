using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;


namespace ExpressYourself
{
    public class VectorStringBuilder
    {
        private const string DOUBLE_FORMAT = "{0:0.0000}";

        public static string BuildString(double X, double Y, double Z, double magnitude)
        {
            string built = "";

            Vector3D vector = new Vector3D(X, Y, Z);
            vector = Vector3D.Multiply(vector, magnitude);

            built += String.Format(DOUBLE_FORMAT, vector.X);
            built += "|";
            built += String.Format(DOUBLE_FORMAT, vector.Y);
            built += "|";
            built += String.Format(DOUBLE_FORMAT, vector.Z);

            return built;
        }
    }
}
