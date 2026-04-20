using OpenTK.Graphics.OpenGL;
using System;

namespace FxEngine.Loaders.Collada
{
    public abstract class Uniform
    {

        private static int NOT_FOUND = -1;

        private String name;
        private int location;

        protected Uniform(String name)
        {
            this.name = name;
        }

        public virtual void storeUniformLocation(int programID)
        {
            location = GL.GetUniformLocation(programID, name);
            if (location == NOT_FOUND)
            {
                throw new Exception("No uniform variable called " + name + " found!");
            }
        }

        protected int getLocation()
        {
            return location;
        }

    }
}


