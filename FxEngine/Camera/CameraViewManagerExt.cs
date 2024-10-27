using OpenTK;
using OpenTK.Mathematics;
using System;
using System.Drawing;
using System.Windows.Forms;
using FxEngine.Gui;
using OpenTK.GLControl;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace FxEngine.Cameras
{
    public class CameraViewManagerExt : CameraViewManager
    {
        public override void Update()
        {
            var dir = Camera.CamFrom - Camera.CamTo;
            var cv = dir;
            var a1 = Vector3.Cross(Camera.CamUp, cv.Normalized()); ;            
            var moveVecTan = a1.Normalized();
            var moveVec = Vector3.Cross(a1.Normalized(), cv.Normalized()).Normalized();

            var pos = CursorPosition;

            if (drag2)
            {
                var zoom = Width / Camera.OrthoWidth;

                var dx = moveVecTan * ((startPosX - pos.X) / zoom) + moveVec * ((startPosY - pos.Y) / zoom);
                Camera.CamFrom = cameraFromStart + dx;
                Camera.CamTo = cameraToStart + dx;
            }
            if (drag)
            {
                //rotate here
                float kk = 3;                
                Vector3 v1 = cameraFromStart - cameraToStart;

                var m1 = Matrix3.CreateFromAxisAngle(Vector3.Cross(v1, cameraUpStart), -(startPosY - pos.Y) / 180f / kk * (float)Math.PI);
                var m2 = Matrix3.CreateFromAxisAngle(cameraUpStart, -(startPosX - pos.X) / 180f / kk * (float)Math.PI);                

                v1 *= m1;
                v1 *= m2;
                var up1 = cameraUpStart;

                Camera.CamUp = up1;                
                Camera.CamFrom = cameraToStart + v1;              
            }
        }

        public float AlongRotate = 0;
        public Camera Camera;

        public override void Attach(BaseGlDrawingContext ctx, Camera camera)
        {
            if (ctx is GlDrawingContext c)
            {
                Attach(c.GameWindow, camera);
            }
            if (ctx is GlControlDrawingContext cc)
            {
                Attach(cc.GameWindow, camera);
            }
        }

        public override void Attach(GameWindow control, Camera camera)
        {
            base.Attach(control, camera);
            Camera = camera;
            control.MouseUp += Control_MouseUp;
            //control.MouseDown += Control_MouseDown;

            control.KeyUp += Control_KeyUp;
            control.KeyDown += Control_KeyDown;
            control.MouseWheel += Control_MouseWheel;
        }

        public override void Attach(GLControl control, Camera camera)
        {
            base.Attach(control, camera);
            Camera = camera;
            control.MouseUp += Control_MouseUp1;
            //control.MouseDown += Control_MouseDown;

            control.KeyUp += Control_KeyUp1;
            control.KeyDown += Control_KeyDown1;
            control.MouseWheel += Control_MouseWheel1;
        }

        private void Control_MouseWheel1(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MouseWheel(e.Delta);
        }

        private void Control_KeyDown1(object sender, KeyEventArgs e)
        {
            if (e.Shift)
            {
                lshift = true;
            }
        }

        private void Control_KeyUp1(object sender, KeyEventArgs e)
        {
            lshift = false;
        }

        private void Control_MouseUp1(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            drag = false;
            drag2 = false;
        }
        public override void Deattach(BaseGlDrawingContext ctx)
        {
            if (ctx is GlDrawingContext c)
            {
                Deattach(c.GameWindow);
            }
            if (ctx is GlControlDrawingContext cc)
            {
                Deattach(cc.GameWindow);
            }
        }
        public override void Deattach(GLControl control)
        {

        }
        public override void Deattach(GameWindow control)
        {
            control.MouseUp -= Control_MouseUp;
            control.KeyUp -= Control_KeyUp;
            control.KeyDown -= Control_KeyDown;
            control.MouseWheel -= Control_MouseWheel;
        }

        private void Control_MouseWheel(MouseWheelEventArgs e)
        {
            if (!Enable) 
                return;

            //MouseWheel(e.Delta);
            MouseWheel((int)e.OffsetY);
        }

        public Point PointToClient(Point p)
        {
            if (GameWindow != null)
            {
                return GameWindow.PointToClient(p.ToVector2i()).ToPoint();
            }
            return GlControl.PointToClient(p);
        }

        public void MakeCurrent()
        {
            if (GameWindow != null)
            {
                GameWindow.MakeCurrent();
                return;
            }
            GlControl.MakeCurrent();
        }

        public int Width
        {
            get
            {
                if (GameWindow != null)
                {
                    return GameWindow.Width();
                }
                return GlControl.Width;
            }
        }

        public Rectangle ClientRectangle
        {
            get
            {
                if (GameWindow != null)
                {
                    return GameWindow.ClientRectangle.ToRectangle();
                }
                return GlControl.ClientRectangle;
            }
        }

        public void UpdateMatricies(Camera cam)
        {
            if (GameWindow != null)
            {
                cam.UpdateMatricies(GameWindow);
                return;
            }
            cam.UpdateMatricies(GlControl);
        }

        public void MouseWheel(int delta)
        {
            float zoomK = 20;
            var cur = PointToClient(Cursor.Position);
            MakeCurrent();            
            MouseRay mr = new MouseRay(cur.X, cur.Y, Camera);            

            var camera = Camera;
            if (camera.IsOrtho)
            {
                var shift = mr.Start - Camera.CamFrom;
                shift.Normalize();                
                if (delta > 0)
                {
                    camera.OrthoWidth /= 1.2f;                    
                    Camera cam2 = new Camera();
                    cam2.CamFrom = camera.CamFrom;
                    cam2.CamTo = camera.CamTo;
                    cam2.CamUp = camera.CamUp;
                    cam2.OrthoWidth = camera.OrthoWidth;
                    cam2.IsOrtho = camera.IsOrtho;

                    UpdateMatricies(cam2);
                    MouseRay mr2 = new MouseRay(cur.X, cur.Y, cam2);                    
                    var diff = mr.Start - mr2.Start;
                    shift *= diff.Length;
                    camera.CamFrom += shift;
                    camera.CamTo += shift;
                }
                else
                {
                    camera.OrthoWidth *= 1.2f;
                    
                    Camera cam2 = new Camera();
                    cam2.CamFrom = camera.CamFrom;
                    cam2.CamTo = camera.CamTo;
                    cam2.CamUp = camera.CamUp;
                    cam2.OrthoWidth = camera.OrthoWidth;
                    cam2.IsOrtho = camera.IsOrtho;

                    UpdateMatricies(cam2);
                    MouseRay mr2 = new MouseRay(cur.X, cur.Y, cam2);

                    var diff = mr.Start - mr2.Start;
                    shift *= diff.Length;
                    camera.CamFrom -= shift;
                    camera.CamTo -= shift;
                }

                return;
            }
            if (
                ClientRectangle.IntersectsWith(new Rectangle(PointToClient(Cursor.Position),
                    new System.Drawing.Size(1, 1))))
            {
                var dir = mr.Dir;
                dir.Normalize();
                if (delta > 0)
                {
                    camera.CamFrom += dir * zoomK;
                    camera.CamTo += dir * zoomK;
                }
                else
                {
                    camera.CamFrom -= dir * zoomK;
                    camera.CamTo -= dir * zoomK;
                }
            }
        }

        private void Control_KeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Shift)
            {
                lshift = true;
            }
        }

        private void Control_KeyUp(KeyboardKeyEventArgs e)
        {
            lshift = false;
        }
        
        public static Vector3? lineIntersection(Vector3 planePoint, Vector3 planeNormal, Vector3 linePoint, Vector3 lineDirection)
        {
            if (Math.Abs(Vector3.Dot(planeNormal, lineDirection)) < 10e-6f)
            {
                return null;
            }

            var dot1 = Vector3.Dot(planeNormal, planePoint);
            var dot2 = Vector3.Dot(planeNormal, linePoint);
            var dot3 = Vector3.Dot(planeNormal, lineDirection);
            double t = (dot1 - dot2) / dot3;
            return linePoint + lineDirection * (float)t;
        }

        public bool SnapMode = false;
        public bool SnapModePlane = false;
        public void Control_MouseDown(MouseButtonEventArgs e)
        {
            var ee = new LocalMouseEventState();
            ee.IsLeftPressed = e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left && e.IsPressed;
            ee.IsRightPressed = e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Right && e.IsPressed;
            ee.IsMiddlePressed = e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Middle && e.IsPressed;
            MouseDown(ee);
        }

        public void Control_MouseDown1(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var ee = new LocalMouseEventState();
            ee.IsLeftPressed = e.Button == MouseButtons.Left;
            ee.IsRightPressed = e.Button == MouseButtons.Right;
            ee.IsMiddlePressed = e.Button == MouseButtons.Middle;
            MouseDown(ee);
        }

        public void MouseDown(LocalMouseEventState e)
        {
            var pos = CursorPosition;
            startPosX = pos.X;
            startPosY = pos.Y;
            cameraFromStart = Camera.CamFrom;
            cameraToStart = Camera.CamTo;
            cameraUpStart = Camera.CamUp;

            if (e.IsMiddlePressed)
            {                
                var d1 = Camera.CamFrom - Camera.CamTo;
                //var plane1 : forw
                var crs1 = Vector3.Cross(cameraUpStart, d1);                
                if (SnapModePlane)
                {
                    var inter = lineIntersection(Vector3.Zero, Vector3.UnitZ, Camera.CamFrom, Camera.CamTo - Camera.CamFrom);
                    if (inter != null)
                    {
                        drag = true;
                        Camera.CamTo = inter.Value;
                        cameraToStart = Camera.CamTo;
                    }
                }
                else if (SnapMode)
                {
                    var inter = lineIntersection(Camera.CamTo, crs1, Vector3.Zero, Vector3.UnitX);
                    if (inter != null)
                    {
                        drag = true;
                        Camera.CamTo = inter.Value;
                        cameraToStart = Camera.CamTo;
                    }
                }
                else
                {
                    drag = true;
                }                
            }

            if (e.IsRightPressed)
            {
                drag2 = true;                
            }
        }

        bool lshift = false;

        float startPosX;
        float startPosY;
        Vector3 cameraFromStart;
        Vector3 cameraToStart;
        Vector3 cameraUpStart;
        public PointF CursorPosition
        {
            get
            {
                if (GameWindow != null)
                {
                    return GameWindow.PointToClient(Cursor.Position.ToVector2i()).ToPoint();
                }
                return GlControl.PointToClient(Cursor.Position);
            }
        }
        bool drag = false;
        public bool drag2 = false;

        private void Control_MouseUp(MouseButtonEventArgs e)
        {
            drag = false;
            drag2 = false;
        }
    }

}
