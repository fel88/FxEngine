using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxEngine
{
    public class LightSource
    {
        public bool IsEnabled { get; set; } = true;
        public Vector4 Position { get; set; } = new Vector4(0, 0, 100, 0);
        public float[] light_ambient { get; set; } = { 0.5f, 0.5f, 0.5f, 1.0f };
        public float[] light_diffuse { get; set; } = { 1.0f, 1.0f, 1.0f, 1.0f };

        public void Setup(int position)
        {
            if (!IsEnabled) {
                GL.Disable(EnableCap.Light0 + position);
                return; }
            var num = LightName.Light0 + position;
            GL.Enable(EnableCap.Light0 + position);
            GL.Light(num, LightParameter.Diffuse, light_diffuse);
            //GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
            //GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Light(num, LightParameter.Position, Position);
            GL.Light(num, LightParameter.Ambient, light_ambient);
        }
    }

}
