using FxEngine.Tiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxEngine.Gui
{
    public class GlButton : GlGuiElement
    {
        public object Tag;

        public Size Size;
        public Tile Tile;
        public PointF Position;
        public override void Draw(BaseGlDrawingContext dc)
        {
            Tile.Position = Position;
            Tile.ForceQuadSize = Size;
            Tile.Draw();
            Tile.ForceQuadSize = null;
        }

        public override void Update(BaseGlDrawingContext dc)
        {

        }
    }

   
}
