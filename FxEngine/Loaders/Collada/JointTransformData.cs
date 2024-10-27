using OpenTK.Mathematics;
using System;

namespace FxEngine.Loaders.Collada
{
    public class JointTransformData
    {

        public string jointNameId;
        public Matrix4 jointLocalTransform;

        public JointTransformData(String jointNameId, Matrix4 jointLocalTransform)
        {
            this.jointNameId = jointNameId;
            this.jointLocalTransform = jointLocalTransform;
        }
    }
}


