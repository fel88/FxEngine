using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxEngine.Gui
{
    public abstract class GlGuiElement
    {
        public string Id;
        public bool Enabled { get; set; } = true;
        public int ZOrder;
        public virtual bool ContainFocus { get { return Focused; } }
        public bool Focused { get; set; }
        public object Tag { get; set; }
        public virtual bool Visible { get; set; } = true;
        public GlGuiElement Parent;
        public GlGuiTextAlign TextAlign;

        public GuiBounds Rect = new GuiBounds();
        public abstract void Draw(BaseGlDrawingContext dc);
        public abstract void Update(BaseGlDrawingContext dc);
        public virtual void Event(BaseGlDrawingContext dc, GlGuiEvent ev) { }

        public virtual void ResetFocus()
        {
            Focused = false;
        }
    }

    public class GlGuiEvent
    {
        public bool FocusChanged;
        public GlGuiElement NewFocusElement;
        public Point Position;
        public bool Handled;
    }

    public class MouseClickGlGuiEvent : GlGuiEvent
    {
        public bool IsLeft;
    }

    public class KeyGlGuiEvent : GlGuiEvent
    {
        public Key Key;
        public KeyEventTypeEnum Type;
        public enum KeyEventTypeEnum
        {
            Down, Up, Pressed
        }
    }

    public enum GlGuiTextAlign
    {
        Left, Center
    }

}
