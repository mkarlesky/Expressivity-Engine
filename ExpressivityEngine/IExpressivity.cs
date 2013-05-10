using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;

namespace ExpressivityEngine
{
    public interface IExpressivity
    {
        void ExpressivityUpdate(double X, double Y, double Z, double magnitude);
    }
}
