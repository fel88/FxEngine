using OpenTK.Input;

namespace FxEngine.Gui
{
    public class KeyGlGuiEvent : GlGuiEvent
    {
        public Key Key;
        public KeyEventTypeEnum Type;
        public enum KeyEventTypeEnum
        {
            Down, Up, Pressed
        }
    }

}
