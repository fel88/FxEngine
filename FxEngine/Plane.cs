using OpenTK;
using OpenTK.Mathematics;

namespace FxEngine
{
    public class Plane
    {
        private Vector3d _normal;

        public Vector3d Normal
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

        public Plane(Vector3d normal, double w)
        {
            _normal = normal;
            _w = w;
        }
        public static Plane FromPoints(Vector3d a, Vector3d b, Vector3d c)
        {
            var n = Vector3d.Cross(b - a, c - a).Normalized();
            return new Plane(n, Vector3d.Dot(n, a));
        }


        public override string ToString()
        {
            return string.Format("{0} {1}", _normal, _w);
        }
    }

}

