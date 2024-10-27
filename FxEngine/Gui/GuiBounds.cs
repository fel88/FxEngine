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

        public GuiBounds(float l, float t, float w, float h, GuiAnchor anchor = GuiAnchor.Left) : base()
        {
            XOffset = l;
            YOffset = t;
            Width = w;
            Height = h;
            Anchor = anchor;
        }        

        public float XOffset { get; set; }
        public float YOffset { get; set; }

        public void Update()
        {
            if (Parent == null) return;
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
        public float Left { get; set; }

        public float Top { get; set; }

        public float Width;
        public float Height;

        public float Right { get { return Left + Width; } }
        public float Bottom { get { return Top + Height; } }

        public bool IntersectsWith(float x, float y)
        {
            return x >= Left && x <= Right && y >= Top && y <= Bottom;
        }

        public bool IntersectsWith(Vector2 point)
        {
            return IntersectsWith(point.X, point.Y);
        }
    }
    
}
