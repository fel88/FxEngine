using OpenTK.Mathematics;
using System.Drawing;

namespace FxEngine.Tiles
{
    public class TileDrawItem
    {
        public float Z;
        public float Scale = 1;
        public Tile Tile;
        public PointF IsometricPosition;
        public Vector3 Position3;
        public PointF Position
        {
            get
            {
                return new PointF(Position3.X, Position3.Y);
            }
            set
            {
                Position3.X = value.X;
                Position3.Y = value.Y;
                Position3.Z = 0;
            }
        }
    }
}
