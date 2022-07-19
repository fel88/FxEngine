using System;

namespace FxEngine.Loaders.Collada
{
    public class DisplayManager
    {
        private static DateTime lastFrameTime;
        private static float delta;
        public static void update()
        {

            var currentFrameTime = DateTime.Now;
            delta = (float)(currentFrameTime - lastFrameTime).TotalMilliseconds / 1000f;
            lastFrameTime = currentFrameTime;
        }

        public static float getFrameTime()
        {
            return delta;
        }


    }
}


