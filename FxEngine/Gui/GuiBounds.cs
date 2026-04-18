using OpenTK.Mathematics;


namespace FxEngine.Gui
{
    public class GuiBounds
    {
        public GuiAnchor Anchor;
        public GuiBounds Parent;
        public static GuiBounds DefaultBound;

        public GuiBounds()
        {
            Parent = DefaultBound;
        }

        public GuiBounds(double l, double t, double w, double h, GuiAnchor anchor = GuiAnchor.Left) : base()
        {
            XOffset = l;
            YOffset = t;
            Width = w;
            Height = h;
            Anchor = anchor;
        }        

        public double XOffset { get; set; }
        public double YOffset { get; set; }

        public void Update()
        {
            if (Parent == null) 
                return;

            if ((GuiAnchor.Right & (Anchor)) > 0)
            {
                Left = Parent.Right - XOffset;
            }
            else
            {
                Left = Parent.Left + XOffset;
            }
            if ((GuiAnchor.CenterX & (Anchor)) > 0)
            {
                Left = Parent.Left + Parent.Width / 2 + XOffset;
            }
            if ((GuiAnchor.Bottom & (Anchor)) > 0)
            {
                Top = Parent.Bottom - YOffset;
            }
            else
            {
                Top = Parent.Top + YOffset;
            }
            if ((GuiAnchor.CenterY & (Anchor)) > 0)
            {
                Top = Parent.Top + Parent.Height / 2 + YOffset;
            }
        }
        public double Left { get; set; }

        public double Top { get; set; }

        public double Width;
        public double Height;

        public double Right => Left + Width;
        public double Bottom => Top + Height;

        public bool IntersectsWith(double x, double y)
        {
            return x >= Left && x <= Right && y >= Top && y <= Bottom;
        }

        public bool IntersectsWith(Vector2 point)
        {
            return IntersectsWith(point.X, point.Y);
        }
    }
    
}
