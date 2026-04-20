using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FxEngine.Gui
{
    public class NativeGlGuiElement : GlGuiElement
    {

        public NativeGlGuiElement()
        {
            Rect.Parent = Drawer.CurrentBound;
        }
        public GuiAnchor Anchor
        {
            get
            {
                return Rect.Anchor;
            }
            set
            {
                Rect.Anchor = value;
            }
        }
        public static NativeDrawProvider Drawer;
        public override void Draw(BaseGlDrawingContext dc)
        {

        }

        public override void Update(BaseGlDrawingContext dc)
        {

        }
    }



}
