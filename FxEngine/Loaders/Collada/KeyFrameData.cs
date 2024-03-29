﻿using System.Collections.Generic;

namespace FxEngine.Loaders.Collada
{
    public class KeyFrameData
    {

        public float time;
        public List<JointTransformData> jointTransforms = new List<JointTransformData>();

        public KeyFrameData(float time)
        {
            this.time = time;
        }

        public void addJointTransform(JointTransformData transform)
        {
            jointTransforms.Add(transform);
        }

    }
}


