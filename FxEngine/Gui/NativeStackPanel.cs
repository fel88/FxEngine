namespace FxEngine.Gui
{
    public class NativeStackPanel : NativePanel
    {        
        public override void Update(BaseGlDrawingContext dc)
        {
            float h = 0;
            float yy = 30;
            foreach (var item in Childs)
            {
                h += item.Rect.Height;
                item.Rect.YOffset = yy;
                yy += item.Rect.Height;
            }

            Rect.Height = h + 35;
            base.Update(dc);
        }
    }
}
