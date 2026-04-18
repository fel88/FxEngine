using FxEngine.Interfaces;
using OpenTK.Windowing.Desktop;

namespace FxEngine.Cameras
{
    public class GameWindowGameControlWrapper : IGameControlWrapper
    {
        public readonly GameWindow Control;
        public GameWindowGameControlWrapper(GameWindow control)
        {
            Control = control;
        }

        public int Height()
        {
            return Control.Height();
        }

        public int Width()
        {
            return Control.Width();
        }
    }
}

