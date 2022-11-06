namespace FxEngine.Gui
{
    public interface IGuiElement
    {
        void Draw(BaseGlDrawingContext dc);
        void Update(BaseGlDrawingContext dc);
        void Event(BaseGlDrawingContext dc, GlGuiEvent ev);
        string Id { get; }
        bool Enabled { get; set; }
        int ZOrder { get; set; }
        bool ContainFocus { get; }
        GuiBounds Rect { get; set; }
        void ResetFocus();
        bool Visible { get; set; }
        GlGuiTextAlign TextAlign { get; set; }

    }

}
