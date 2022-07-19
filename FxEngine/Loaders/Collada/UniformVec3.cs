using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace FxEngine.Loaders.Collada
{
    public class UniformVec3 : Uniform
    {

        private float currentX;
        private float currentY;
        private float currentZ;
        private bool used = false;

        public UniformVec3(String name) : base(name)
        {

        }

        public void loadVec3(Vector3 vector)
        {
            loadVec3(vector.X, vector.Y, vector.Z);
        }

        public void loadVec3(float x, float y, float z)
        {
            if (!used || x != currentX || y != currentY || z != currentZ)
            {
                this.currentX = x;
                this.currentY = y;
                this.currentZ = z;
                used = true;
                GL.Uniform3(getLocation(), x, y, z);
            }
        }

    }
}


