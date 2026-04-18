using FxEngine.Interfaces;
using OpenTK.GLControl;

namespace FxEngine.Cameras
{
    public class GlControlGameControlWrapper : IGameControlWrapper
    {
        public readonly GLControl Control;
        public GlControlGameControlWrapper(GLControl control)
        {
            Control = control;
        }

        public int Height()
        {
            return Control.Height;
        }

        public int Width()
        {
            return Control.Width;
        }
    }
}

