using OpenTK.Mathematics;

namespace FxEngine.Loaders.Collada
{
    public class AnimatedModel
    {
        private Vao model;
        private TextureLocal texture;

        // skeleton
        private ColladaJoint rootJoint;
        private int jointCount;

        private Animator animator;

        public AnimatedModel(Vao model, TextureLocal texture, ColladaJoint rootJoint, int jointCount)
        {
            this.model = model;
            this.texture = texture;
            this.rootJoint = rootJoint;
            this.jointCount = jointCount;
            this.animator = new Animator(rootJoint);
            rootJoint.CalcInverseBindTransform(new Matrix4());
        }


        public Vao getModel()
        {
            return model;
        }


        public TextureLocal getTexture()
        {
            return texture;
        }


        public ColladaJoint getRootJoint()
        {
            return rootJoint;
        }


        /*  public void delete()
          {
              model.delete();
              texture.delete();
          }*/


        public void doAnimation(Animation animation)
        {
            animator.doAnimation(animation);
        }


        public void update()
        {
            animator.update();
        }


        public Matrix4[] getJointTransforms()
        {
            Matrix4[] jointMatrices = new Matrix4[jointCount];
            addJointsToArray(rootJoint, jointMatrices);
            return jointMatrices;
        }


        private void addJointsToArray(ColladaJoint headJoint, Matrix4[] jointMatrices)
        {
            jointMatrices[int.Parse(headJoint.Id)] = headJoint.AnimatedTransform;
            foreach (ColladaJoint childJoint in headJoint.Childs)
            {
                addJointsToArray(childJoint, jointMatrices);
            }
        }
    }
}


