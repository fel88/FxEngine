using OpenTK.Graphics.OpenGL;
using System;

namespace FxEngine.Loaders.Collada
{
    public class UniformSampler : Uniform
    {


        private int currentValue;
        private bool used = false;

        public UniformSampler(String name) : base(name)
        {

        }

        public void loadTexUnit(int texUnit)
        {
            if (!used || currentValue != texUnit)
            {
                GL.Uniform1(getLocation(), texUnit);
                used = true;
                currentValue = texUnit;
            }
        }

    }
}


