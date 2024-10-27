using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using FxEngine.Gui;
using OpenTK.Windowing.Desktop;
using OpenTK.GLControl;

namespace FxEngine.Cameras
{
    public class Camera
    {
        public float zoom = 1;
        public void UpdateMatricies(BaseGlDrawingContext ctx)
        {
            if (ctx is GlControlDrawingContext c)
            {
                UpdateMatricies(c.GameWindow);
            }
            if (ctx is GlDrawingContext cс)
            {
                UpdateMatricies(cс.GameWindow);
            }
        }
        public void UpdateMatricies(OpenTK.Windowing.Desktop.GameWindow glControl)
        {

            viewport[0] = 0;
            viewport[1] = 0;
            viewport[2] = glControl.Width();
            viewport[3] = glControl.Height();
            var aspect = glControl.Width() / (float)glControl.Height();
            var o = Matrix4.CreateOrthographic(OrthoWidth, OrthoWidth / aspect, -25e4f, 25e4f);

            Matrix4 mp = Matrix4.CreatePerspectiveFieldOfView((float)(Fov * Math.PI / 180) * zoom,
                glControl.Width() / (float)glControl.Height(), 1, 25e4f);


            if (IsOrtho)
            {
                ProjectionMatrix = o;

            }
            else
            {
                ProjectionMatrix = mp;

            }

            OpenTK.Mathematics.Matrix4 modelview = OpenTK.Mathematics.Matrix4.LookAt(CamFrom, CamTo, CamUp);
            ViewMatrix = modelview;
        }

        public void UpdateMatricies(OpenTK.GLControl.GLControl glControl)
        {

            viewport[0] = 0;
            viewport[1] = 0;
            viewport[2] = glControl.Width;
            viewport[3] = glControl.Height;
            var aspect = glControl.Width / (float)glControl.Height;
            var o = Matrix4.CreateOrthographic(OrthoWidth, OrthoWidth / aspect, -25e4f, 25e4f);

            Matrix4 mp = Matrix4.CreatePerspectiveFieldOfView((float)(Fov * Math.PI / 180) * zoom,
                glControl.Width / (float)glControl.Height, 1, 25e4f);


            if (IsOrtho)
            {
                ProjectionMatrix = o;

            }
            else
            {
                ProjectionMatrix = mp;

            }

            Matrix4 modelview = Matrix4.LookAt(CamFrom, CamTo, CamUp);
            ViewMatrix = modelview;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public Vector3 CamFrom { get; set; } = new Vector3(70, 70, 70);

        public Vector3 CamTo { get; set; } = new Vector3(0, 0, 0);
        public Vector3 CamUp { get; set; } = new Vector3(0, 0, 1);

        public bool IsOrtho { get; set; } = false;
        public float Fovy { get; set; } = 60;
        public float Aspect { get; private set; }
        public Vector3 Direction
        {
            get
            {
                return CamFrom - CamTo;
            }
        }

        public Matrix4 ProjectionMatrix { get; set; }
        public Matrix4 ViewMatrix { get; set; }
        public int[] viewport = new int[4];

        public void SetupCore(GameWindow glControl)
        {
            GL.Viewport(0, 0, glControl.Width(), glControl.Height());
            { var er = GL.GetError(); }
            viewport[0] = 0;
            viewport[1] = 0;
            viewport[2] = glControl.Width();
            viewport[3] = glControl.Height();
            var aspect = glControl.Width() / (float)glControl.Height();
            var o = Matrix4.CreateOrthographic(OrthoWidth, OrthoWidth / aspect, -25e4f, 25e4f);

            Matrix4 mp = Matrix4.CreatePerspectiveFieldOfView((float)(Fov * Math.PI / 180) * zoom,
                glControl.Width() / (float)glControl.Height(), 1, 25e4f);


            if (IsOrtho)
            {
                ProjectionMatrix = o;
            }
            else
            {
                ProjectionMatrix = mp;
            }

            Matrix4 modelview = Matrix4.LookAt(CamFrom, CamTo, CamUp);
            ViewMatrix = modelview;
        }

        public void Setup(BaseGlDrawingContext ctx)
        {
            if (ctx is GlControlDrawingContext c)
            {
                Setup(c.GameWindow);
            }
            if (ctx is GlDrawingContext cc)
            {
                Setup(cc.GameWindow);
            }
        }
        public void Setup(GameWindow glControl)
        {

            GL.Viewport(0, 0, glControl.Width(), glControl.Height());

            viewport[0] = 0;
            viewport[1] = 0;
            viewport[2] = glControl.Width();
            viewport[3] = glControl.Height();
            var aspect = glControl.Width() / (float)glControl.Height();
            var o = Matrix4.CreateOrthographic(OrthoWidth, OrthoWidth / aspect, -25e4f, 25e4f);

            Matrix4 mp = Matrix4.CreatePerspectiveFieldOfView((float)(Fov * Math.PI / 180) * zoom,
                glControl.Width() / (float)glControl.Height(), 1, 25e4f);

            GL.MatrixMode(MatrixMode.Projection);

            if (IsOrtho)
            {
                ProjectionMatrix = o;
                GL.LoadMatrix(ref o);
            }
            else
            {
                ProjectionMatrix = mp;
                GL.LoadMatrix(ref mp);
            }


            Matrix4 modelview = Matrix4.LookAt(CamFrom, CamTo, CamUp);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            ViewMatrix = modelview;
        }

        public void Setup(GLControl gl)
        {

            GL.Viewport(0, 0, gl.Width, gl.Height);
            viewport[0] = 0;
            viewport[1] = 0;
            viewport[2] = gl.Width;
            viewport[3] = gl.Height;
            var aspect = gl.Width / (float)gl.Height;
            var o = Matrix4.CreateOrthographic(OrthoWidth, OrthoWidth / aspect, -25e4f, 25e4f);

            Matrix4 mp = Matrix4.CreatePerspectiveFieldOfView((float)(Fov * Math.PI / 180) * zoom,
                gl.Width / (float)gl.Height, 1, 25e4f);

            GL.MatrixMode(MatrixMode.Projection);
            if (IsOrtho)
            {
                o = Matrix4.CreateOrthographic(gl.Width * OrthoZoom, gl.Height * OrthoZoom, -1000, 100000);
                ProjectionMatrix = o;
                GL.LoadMatrix(ref o);
            }
            else
            {
                ProjectionMatrix = mp;
                GL.LoadMatrix(ref mp);
            }

            Matrix4 modelview = Matrix4.LookAt(CamFrom, CamTo, CamUp);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            ViewMatrix = modelview;            
        }
        public float OrthoZoom = 1;

        public float OrthoWidth { get; set; } = 1000;
        public float Fov { get; set; } = 60;

        public void SetupViewOnly()
        {
            Matrix4 modelview = Matrix4.LookAt(CamFrom, CamTo, CamUp);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
        }

        public void CopyFrom(Camera cam)
        {
            CamFrom = cam.CamFrom;
            CamTo = cam.CamTo;
            CamUp = cam.CamUp;
            IsOrtho = cam.IsOrtho;
        }

        public Matrix4 GetBillboardMatrix(Vector3 pos)
        {
            var pm = ViewMatrix;
            var m = new Matrix4(
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
