using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FxEngine
{
    public class IntersectStuff
    {
        public static IntersectInfo CheckIntersect(MouseRay ray, DrawPolygon[] poly)
        {
            List<IntersectInfo> intersects = new List<IntersectInfo>();
            if (poly != null)
            {
                foreach (var polygon in poly)
                {
                    var vv = polygon.Vertices.Select(z => z.Position).ToArray();
                    var res = CheckIntersect(ray, vv);
                    if (res.HasValue)
                    {
                        intersects.Add(new IntersectInfo()
                        {
                            Distanse = (res.Value - ray.Start).Length,
                            Target = polygon,
                            Point = res.Value
                        });
                    }
                }
            }
            if (intersects.Any())
            {
                return intersects.OrderBy(z => z.Distanse).First();
            }
            return null;
        }

        public static Vector3? CheckIntersect(MouseRay ray, Vector3[] triangle)
        {            
            var dir = ray.End - ray.Start;
            dir.Normalize();
            
            var a = triangle[0];
            var b = triangle[1];
            var c = triangle[2];
            var plane = Plane.FromPoints(new Vector3(a.X, a.Y, a.Z),
                new Vector3(b.X, b.Y, b.Z),
                new Vector3(c.X, c.Y, c.Z));

            plane.W = -plane.W;
            var s = InstersectPlaneWithRay(plane, ray);

            if (s != null)
            {                
                var ss = s.Value;
                var v1 = triangle[1] - triangle[0];
                var v2 = triangle[2] - triangle[1];
                var v3 = triangle[0] - triangle[2];
                var crs1 = Vector3.Cross(ss - triangle[0], v1);
                var crs2 = Vector3.Cross(ss - triangle[1], v2);
                var crs3 = Vector3.Cross(ss - triangle[2], v3);
                var up = Vector3.Cross(v1, triangle[2] - triangle[0]);
                //find dot
                var dot1 = Vector3.Dot(crs1, up);
                var dot2 = Vector3.Dot(crs2, up);
                var dot3 = Vector3.Dot(crs3, up);
                
                if (Math.Sign(dot1) == Math.Sign(dot2) && Math.Sign(dot2) == Math.Sign(dot3) /*&& Math.Sign(dot1) == 1*/)
                {                    
                    return ss;
                }
            }
            return null;
        }

        public static Vector3? InstersectPlaneWithRay(Plane plane, MouseRay ray, bool checkT = true)
        {
            Vector3 l = ray.End - ray.Start;
            l.Normalize();

            //check point exists 
            var n = plane.Normal;
            var d = l.X * n.X + l.Y * n.Y + n.Z * l.Z;
            var r0n = ray.Start.X * n.X + ray.Start.Y * n.Y + ray.Start.Z * n.Z;
            if (Math.Abs(d) > 1e-4)
            {
                var t0 = -((r0n) + plane.W) / d;
                if (checkT)
                {
                    if (t0 >= 0)
                    {
                        return ray.Start + l * (float)t0;
                    }
                }
                else
                {
                    return ray.Start + l * (float)t0;
                }
            }
            return null;
        }
    }
}