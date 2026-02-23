using OpenTK.Mathematics;
using System;

namespace FxEngine
{
    public class MatrixGenerator
    {
        public Vector3d position;
        float scale;
        float rotZ;

        public double PositionX
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
        public double PositionY
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
        public double PositionZ
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

        public Matrix4d GetMatrix()
        {
            var m = Matrix4d.Identity;
            m *= Matrix4d.CreateScale(Scale);
            m *= Matrix4d.CreateRotationZ(RotationZ * (float)Math.PI / 180.0f);
            m *= Matrix4d.CreateTranslation(position);
            return m;
        }
    }
}

