
namespace FxEngine.Gui
{
    public class KeyGlGuiEvent : GlGuiEvent
    {
        public OpenTK.Windowing.GraphicsLibraryFramework.Keys Key;
        public KeyEventTypeEnum Type;
        public enum KeyEventTypeEnum
        {
            Down, Up, Pressed
        }
    }

}
