using OpenTK;

namespace FxEngine.Loaders.Collada
{
    public class JointTransform
    {


        private Vector3 position;
        private Quaternion rotation;


        public JointTransform(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }


        public Matrix4 getLocalTransform()
        {
            Matrix4 matrix = Matrix4.CreateTranslation(position);


            var rot = Matrix4.CreateFromQuaternion(rotation);
            var ind = rot.Inverted();
            //matrix = Matrix4.Mult(matrix);
            matrix = ind * matrix;
            return matrix;
        }


        public static JointTransform interpolate(JointTransform frameA, JointTransform frameB, float progression)
        {
            Vector3 pos = interpolate(frameA.position, frameB.position, progression);
            Quaternion rot = qinterpolate(frameA.rotation, frameB.rotation, progression);
            return new JointTransform(pos, rot);
        }

        public static Quaternion qinterpolate(Quaternion a, Quaternion b, float blend)
        {
            Quaternion result = new Quaternion(0, 0, 0, 1);
            float dot = a.W * b.W + a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            float blendI = 1f - blend;
            if (dot < 0)
            {
                result.W = blendI * a.W + blend * -b.W;
                result.X = blendI * a.X + blend * -b.X;
                result.Y = blendI * a.Y + blend * -b.Y;
                result.Z = blendI * a.Z + blend * -b.Z;
            }
            else
            {
                result.W = blendI * a.W + blend * b.W;
                result.X = blendI * a.X + blend * b.X;
                result.Y = blendI * a.Y + blend * b.Y;
                result.Z = blendI * a.Z + blend * b.Z;
            }
            result.Normalize();
            return result;
        }

        private static Vector3 interpolate(Vector3 start, Vector3 end, float progression)
        {
            float x = start.X + (end.X - start.X) * progression;
            float y = start.Y + (end.Y - start.Y) * progression;
            float z = start.Z + (end.Z - start.Z) * progression;
            return new Vector3(x, y, z);
        }

    }
}


