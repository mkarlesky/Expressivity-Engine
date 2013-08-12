using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace ExpressYourself
{
    public class SkeletonDispatcher
    {
        public delegate void SkeletonHandlerDelegate(Skeleton[] skeletons);

        private SkeletonHandlerDelegate _skeletonHandlerDelegates = null;

        public void RegisterSkeletonHandler(SkeletonHandlerDelegate handler)
        {
            _skeletonHandlerDelegates += handler;
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        public void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);

                    _skeletonHandlerDelegates(skeletons);
                }
            }
        }

        static public bool IsSkeletonValid(Skeleton skeleton, SkeletonTrackingMode mode)
        {
            if (mode == SkeletonTrackingMode.Seated)
            {
                foreach (JointType type in KinectManager.SeatedJointList)
                {
                    if (skeleton.Joints[type].TrackingState == JointTrackingState.NotTracked)
                        return false;
                }
            }
            else if (mode == SkeletonTrackingMode.Default)
            {
                foreach (JointType type in KinectManager.DefaultPartialJointList)
                {
                    if (skeleton.Joints[type].TrackingState == JointTrackingState.NotTracked)
                        return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
