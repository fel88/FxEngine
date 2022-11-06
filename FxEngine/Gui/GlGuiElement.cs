namespace FxEngine.Gui
{
    public abstract class GlGuiElement : IGuiElement
    {
        public string Id { get; set; }
        public bool Enabled { get; set; } = true;
        public int ZOrder { get; set; }
        public virtual bool ContainFocus { get { return Focused; } }
        public bool Focused { get; set; }
        public object Tag { get; set; }
        public virtual bool Visible { get; set; } = true;
        public GlGuiElement Parent;
        public GlGuiTextAlign TextAlign { get; set; }

        public GuiBounds Rect { get; set; } = new GuiBounds();
        public abstract void Draw(BaseGlDrawingContext dc);
        public abstract void Update(BaseGlDrawingContext dc);
        public virtual void Event(BaseGlDrawingContext dc, GlGuiEvent ev) { }

        public virtual void ResetFocus()
        {
            Focused = false;
        }
    }

}
