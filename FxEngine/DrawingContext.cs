using System.Collections.Generic;
using System.Drawing;

namespace FxEngine
{
    public abstract class DrawingContext
    {
        public DrawingContext()
        {
            Multicolor = true;
            LineColor = Color.Blue;
            PolygonColor = Color.White;
            LineWidth = 3;
            WireframeLineWidth = 1;
            IsModelsDraw = true;
            PolygonOpacity = 0.5f;
        }

        public abstract int Width
        {
            get; set;
        }
        public abstract int Height
        {
            get; set;
        }

        
        public bool IsEnableHelpersDraw = true;
        
        public bool RawDraw;

        public bool Wireframe = true;
        
        public List<object> SelectedEntities = new List<object>();

        public bool Solid = true;

        
        public bool Multicolor { get; set; }

        public float LineWidth { get; set; }
        public float WireframeLineWidth { get; set; }
        public bool IsVoxelsDraw { get; set; }

        public Color LineColor;
        public Color PolygonColor;
        public float PolygonOpacity { get; set; }

        public bool IsModelsDraw { get; set; }
    }
}

