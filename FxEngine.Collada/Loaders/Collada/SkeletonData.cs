namespace FxEngine.Loaders.Collada
{
    public class SkeletonData
    {

        public int jointCount;
        public JointData headJoint;

        public SkeletonData(int jointCount, JointData headJoint)
        {
            this.jointCount = jointCount;
            this.headJoint = headJoint;
        }

    }
}


