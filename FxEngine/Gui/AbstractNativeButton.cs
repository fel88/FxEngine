using System;

namespace FxEngine.Gui
{
    public abstract class AbstractNativeButton : NativeGlGuiElement, IButton
    {
        public override void Event(BaseGlDrawingContext dc, GlGuiEvent ev)
        {
            if (!Enabled) return;
            if (Parent is NativePanel)
            {
                if (!(Parent as NativePanel).IsChildsVisible)
                {
                    return;
                }
            }
            if (!Visible) return;
            IsHovered = false;
            if (Rect.IntersectsWith(ev.Position.X, ev.Position.Y))
            {
                IsHovered = true;
                if (Hovered != null)
                {
                    Hovered();
                }
            }

            if (ev is MouseClickGlGuiEvent)
            {
                if (IsHovered)
                {
                    if (Click != null)
                    {
                        if (ClickSound != null)
                        {
                            AudioStuff.Stuff.Play(ClickSound.Path);
                        }
                        Click();
                    }
                    ev.Handled = true;
                }
            }
            base.Event(dc, ev);
        }
        public bool IsHovered = false;
        public string Caption { get; set; }

        protected BaseGlDrawingContext DrawingContext;
        public Action Click { get; set; }
        public Action Hovered { get; set; }
        public GameSound ClickSound;
        public override void Update(BaseGlDrawingContext dc)
        {

        }
    }
}
