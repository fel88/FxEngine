using OpenTK;

namespace FxEngine
{
    public class Plane
    {
        private Vector3 _normal;

        public Vector3 Normal
        {
            get { return _normal; }
        }
        private double _w;

        public double W
        {
            get { return _w; }
            set { _w = value; }
        }

        
        private static double Epsilon = 1e-5;

        public Plane(Vector3 normal, double w)
        {
            _normal = normal;
            _w = w;
        }
        public static Plane FromPoints(Vector3 a, Vector3 b, Vector3 c)
        {
            var n = Vector3.Cross(b - a, c - a).Normalized();

            return new Plane(n, Vector3.Dot(n, a));
        }


        public override string ToString()
        {
            return string.Format("{0} {1}", _normal, _w);
        }
    }

}

