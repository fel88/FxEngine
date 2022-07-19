using OpenTK;
using System;

namespace FxEngine
{
    public class MatrixGenerator
    {
        public Vector3 position;
        float scale;
        float rotZ;

        public float PositionX
        {
            get
            {
                return position.X;
            }
            set
            {
                position.X = value;
                OnChanged();

            }
        }
        public float PositionY
        {
            get
            {
                return position.Y;
            }
            set
            {
                position.Y = value;
                OnChanged();

            }
        }
        public float PositionZ
        {
            get
            {
                return position.Z;
            }
            set
            {
                position.Z = value;
                OnChanged();

            }
        }
        public float Scale
        {
            get
            {
                return scale;
            }
            set
            {

                scale = value;
                OnChanged();
            }
        }

        public void OnChanged()
        {
            if (Changed != null)
            {
                Changed();
            }
        }
        public Action Changed;
        public float RotationZ
        {
            get
            {
                return rotZ;
            }
            set
            {

                rotZ = value;
                OnChanged();
            }
        }

        public Matrix4 GetMatrix()
        {
            var m = Matrix4.Identity;
            m *= Matrix4.CreateScale(Scale);
            m *= Matrix4.CreateRotationZ(RotationZ * (float)Math.PI / 180.0f);
            m *= Matrix4.CreateTranslation(position);
            return m;
        }
    }
}

