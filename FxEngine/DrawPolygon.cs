namespace FxEngine
{
    public class DrawPolygon
    {
        public object Tag { get; set; }
        public DrawVertex[] Vertices;
        //public PrimitiveType DrawType = PrimitiveType.TriangleFan;
        public bool IsWireframe { get; set; }
    }

}

