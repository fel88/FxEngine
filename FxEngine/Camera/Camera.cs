using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FxEngine.Cameras
{
    public class Camera
    {
        public Camera() { }
        public int Id { get; set; }
        public string Name { get; set; }
        public Vector3d Eye { get; set; } = new Vector3d(70, 70, 70);

        public Vector3d Target { get; set; } = new Vector3d(0, 0, 0);
        public Vector3d Up { get; set; } = new Vector3d(0, 0, 1);

        public bool IsOrtho { get; set; } = false;
        public float Fovy { get; set; } = 60;
        public float Aspect { get; private set; }
        public Vector3d Direction => Eye - Target;
        public Matrix4d WorldMatrix = Matrix4d.Identity;

        public Matrix4d ProjectionMatrix { get; set; }
        public Matrix4d ViewMatrix { get; set; }
        public int[] viewport = new int[4];
        public float zoom = 1;
        public float OrthoZoom = 1;
        public float ZNear = -25e4f;
        public float ZFar = 25e4f;

        public double OrthoWidth { get; set; } = 1000;
        public double Fov { get; set; } = 60;
        public Vector3d DirNormalized => (Eye - Target).Normalized();

        public void MoveForw(float ang)
        {
            var vect = Eye - Target;
            Target += new Vector3d(ang, 0, 0);
            Eye = vect + Target;
        }

        public void RotateFromZ(float ang)
        {
            var vect = Eye - Target;
            var m = Matrix4d.CreateFromAxisAngle(Up, ang);
            Eye = Vector3d.TransformVector(vect, m) + Target;
            Up = Vector3d.TransformVector(Up, m);
        }

        public void RotateFromX(float ang)
        {
            var vect = Eye - Target;
            var m = Matrix4d.CreateFromAxisAngle(Vector3d.UnitX, ang);

            Eye = Vector3d.TransformVector(vect, m) + Target;
            Up = Vector3d.TransformVector(Up, m);
        }

        public void RotateFromY(float ang)
        {
            var vect = Eye - Target;

            var cross1 = Vector3d.Cross(vect, Up);
            var m = Matrix4d.CreateFromAxisAngle(cross1, ang);
            //var m = Matrix4.CreateRotationY(ang);

            Eye = Vector3d.TransformVector(vect, m) + Target;
            Up = Vector3d.TransformVector(Up, m);
        }


        public void Shift(Vector3d vector3)
        {
            Eye += vector3;
            Target += vector3;
        }

        public Vector3d GetSide()
        {
            var dirr = Eye - Target;
            var forw = new Vector3d(dirr.X, dirr.Y, 0);
            forw.Normalize();
            var crs = Vector3d.Cross(forw, Up);
            var side = new Vector3d(crs.X, crs.Y, 0);
            side.Normalize();
            return side;
        }

      

        public void FitToPoints(Vector3d[] pnts, int w, int h)
        {
            List<Vector2d> vv = new List<Vector2d>();
            foreach (var vertex in pnts)
            {
                var p = MouseRay.Project(new Vector3d((float)vertex.X, (float)vertex.Y, (float)vertex.Z), ProjectionMatrix, ViewMatrix, WorldMatrix, viewport);
                vv.Add(p.Xy);
            }

            //prjs->xy coords
            var minx = vv.Min(z => z.X);
            var maxx = vv.Max(z => z.X);
            var miny = vv.Min(z => z.Y);
            var maxy = vv.Max(z => z.Y);


            var dx = (maxx - minx);
            var dy = (maxy - miny);

            var cx = dx / 2;
            var cy = dy / 2;
            var dir = Target - Eye;
            //center back to 3d

            var mr = new MouseRay(cx + minx, cy + miny, this);
            var v0 = mr.Start;

            Eye = v0;
            Target = Eye + dir;

            var aspect = w / (float)(h);

            dx /= w;
            dx *= OrthoWidth;
            dy /= h;
            dy *= OrthoWidth;

            OrthoWidth = Math.Max(dx, dy);
        }

        public void UpdateMatricies(Vector2i size)
        {
            UpdateMatricies(new Size(size.X, size.Y));
        }

        public void UpdateMatricies(Size size)
        {
            viewport[0] = 0;
            viewport[1] = 0;
            viewport[2] = size.Width;
            viewport[3] = size.Height;
            var aspect = size.Width / (float)size.Height;

            var o = Matrix4d.CreateOrthographic(OrthoWidth, OrthoWidth / aspect, ZNear, ZFar);

            Matrix4d mp = Matrix4d.CreatePerspectiveFieldOfView((float)(Fov * Math.PI / 180) * zoom,
                size.Width / (float)size.Height, 1, 25e4f);


            ProjectionMatrix = IsOrtho ? o : mp;

            Matrix4d modelview = Matrix4d.LookAt(Eye, Target, Up);
            ViewMatrix = modelview;
        }

     

        public void SetupCore(GameWindow glControl)
        {
            GL.Viewport(0, 0, glControl.Width(), glControl.Height());
            { var er = GL.GetError(); }
            viewport[0] = 0;
            viewport[1] = 0;
            viewport[2] = glControl.Width();
            viewport[3] = glControl.Height();
            var aspect = glControl.Width() / (float)glControl.Height();
            var o = Matrix4d.CreateOrthographic(OrthoWidth, OrthoWidth / aspect, ZNear, ZFar);

            Matrix4d mp = Matrix4d.CreatePerspectiveFieldOfView((float)(Fov * Math.PI / 180) * zoom,
                glControl.Width() / (float)glControl.Height(), 1, 25e4f);


            if (IsOrtho)
            {
                ProjectionMatrix = o;
            }
            else
            {
                ProjectionMatrix = mp;
            }

            Matrix4d modelview = Matrix4d.LookAt(Eye, Target, Up);
            ViewMatrix = modelview;
        }
        
        public void Setup(Vector2i size) => Setup(new Size(size.X, size.Y));

        public void Setup(Size size)
        {
            GL.Viewport(0, 0, size.Width, size.Height);
            viewport[0] = 0;
            viewport[1] = 0;
            viewport[2] = size.Width;
            viewport[3] = size.Height;
            var aspect = size.Width / (float)size.Height;
            var o = Matrix4d.CreateOrthographic(OrthoWidth, OrthoWidth / aspect, ZNear, ZFar);

            Matrix4d mp = Matrix4d.CreatePerspectiveFieldOfView((float)(Fov * Math.PI / 180) * zoom,
                size.Width / (float)size.Height, 1, 25e4f);

            GL.MatrixMode(MatrixMode.Projection);
            if (IsOrtho)
            {
                //o = Matrix4.CreateOrthographic(gl.Width * OrthoZoom, gl.Height * OrthoZoom, -1000, 100000);
                ProjectionMatrix = o;
                GL.LoadMatrix(ref o);
            }
            else
            {
                ProjectionMatrix = mp;
                GL.LoadMatrix(ref mp);
            }

            Matrix4d modelview = Matrix4d.LookAt(Eye, Target, Up);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            ViewMatrix = modelview;
            GL.MultMatrix(ref WorldMatrix);

        }
       

        public void SetupViewOnly()
        {
            Matrix4d modelview = Matrix4d.LookAt(Eye, Target, Up);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            GL.MultMatrix(ref WorldMatrix);

        }

        public void CopyFrom(Camera cam)
        {
            Eye = cam.Eye;
            Target = cam.Target;
            Up = cam.Up;
            IsOrtho = cam.IsOrtho;
        }

        public Matrix4d GetBillboardMatrix(Vector3d pos)
        {
            var pm = ViewMatrix;
            var m = new Matrix4d(
                /*pm.Row0[0], pm.Row0[1], pm.Row0[2], pos.X,
                pm.Row1[0], pm.Row1[1], pm.Row1[2], pos.Y,
                pm.Row2[0], pm.Row2[1], pm.Row2[2], pos.Z,
                0, 0, 0, 1*/
                /*pm.Row0[0], pm.Row1[1], pm.Row2[2], pos.X,
                pm.Row0[0], pm.Row1[1], pm.Row2[2], pos.Y,
                pm.Row0[0], pm.Row1[1], pm.Row2[2], pos.Z,
                0, 0, 0, 1*/
                pm.Row0[0], pm.Row1[0], pm.Row2[0], 0,
             pm.Row0[1], pm.Row1[1], pm.Row2[1], 0,
             pm.Row0[2], pm.Row1[2], pm.Row2[2], 0,
             pos.X, pos.Y, pos.Z, 1
                );
            return m;
            /*
             V.a V.e V.i x
V.b V.f V.j y
V.c V.g V.k z
0   0   0   1
*/
        }
    }


}
