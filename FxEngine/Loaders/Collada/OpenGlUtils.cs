using OpenTK.Graphics.OpenGL;

namespace FxEngine.Loaders.Collada
{
    public class OpenGlUtils
    {

        private static bool cullingBackFace = false;
        private static bool inWireframe = false;
        private static bool isAlphaBlending = false;
        private static bool additiveBlending = false;
        private static bool antialiasing = false;
        private static bool depthTesting = false;

        public static void antialias(bool enable)
        {
            if (enable && !antialiasing)
            {
                GL.Enable(EnableCap.Multisample);
                antialiasing = true;
            }
            else if (!enable && antialiasing)
            {
                GL.Disable(EnableCap.Multisample);
                antialiasing = false;
            }
        }

        /*public static void enableAlphaBlending()
        {
            if (!isAlphaBlending)
            {
                GL11.glEnable(GL11.GL_BLEND);
                GL11.glBlendFunc(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA);
                isAlphaBlending = true;
                additiveBlending = false;
            }
        }*//*

        public static void enableAdditiveBlending()
        {
            if (!additiveBlending)
            {
                GL11.glEnable(GL11.GL_BLEND);
                GL11.glBlendFunc(GL11.GL_SRC_ALPHA, GL11.GL_ONE);
                additiveBlending = true;
                isAlphaBlending = false;
            }
        }*/

        public static void disableBlending()
        {
            if (isAlphaBlending || additiveBlending)
            {
                GL.Disable(EnableCap.Blend);
                isAlphaBlending = false;
                additiveBlending = false;
            }
        }

        public static void enableDepthTesting(bool enable)
        {
            if (enable && !depthTesting)
            {
                GL.Enable(EnableCap.DepthTest);
                depthTesting = true;
            }
            else if (!enable && depthTesting)
            {
                GL.Disable(EnableCap.DepthTest);
                depthTesting = false;
            }
        }

        public static void cullBackFaces(bool cull)
        {
            if (cull && !cullingBackFace)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Back);
                cullingBackFace = true;
            }
            else if (!cull && cullingBackFace)
            {
                GL.Disable(EnableCap.CullFace);
                cullingBackFace = false;
            }
        }
        /*
        public static void goWireframe(bool goWireframe)
        {
            if (goWireframe && !inWireframe)
            {
                GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL11.GL_LINE);
                inWireframe = true;
            }
            else if (!goWireframe && inWireframe)
            {
                GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL11.GL_FILL);
                inWireframe = false;
            }
        }*/

    }
}


