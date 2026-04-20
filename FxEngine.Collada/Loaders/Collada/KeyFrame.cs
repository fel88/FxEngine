using System;
using System.Collections.Generic;

namespace FxEngine.Loaders.Collada
{
    public class KeyFrame
    {
        private float timeStamp;
        private Dictionary<String, JointTransform> pose;


        public KeyFrame(float timeStamp, Dictionary<String, JointTransform> jointKeyFrames)
        {
            this.timeStamp = timeStamp;
            this.pose = jointKeyFrames;
        }


        public float getTimeStamp()
        {
            return timeStamp;
        }


        public Dictionary<String, JointTransform> getJointKeyFrames()
        {
            return pose;
        }

    }
}


