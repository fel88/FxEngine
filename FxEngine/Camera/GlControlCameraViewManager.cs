using OpenTK;
using OpenTK.Mathematics;
using System;
using System.Drawing;
using System.Windows.Forms;
using FxEngine.Gui;
using OpenTK.GLControl;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using FxEngine.Interfaces;

namespace FxEngine.Cameras
{
    public class GlControlCameraViewManager : AbstractGameControlCameraViewManager
    {
        public override void Update()
        {
            var dir = Camera.Eye - Camera.Target;
            var cv = dir;
            var a1 = Vector3d.Cross(Camera.Up, cv.Normalized()); ;            
            var moveVecTan = a1.Normalized();
            var moveVec = Vector3d.Cross(a1.Normalized(), cv.Normalized()).Normalized();

            var pos = CursorPosition;

            if (drag2)
            {
                var zoom = Width / Camera.OrthoWidth;

                var dx = moveVecTan * ((startPosX - pos.X) / zoom) + moveVec * ((startPosY - pos.Y) / zoom);
                Camera.Eye = cameraFromStart + dx;
                Camera.Target = cameraToStart + dx;
            }
            if (drag)
            {
                //rotate here
                float kk = 3;                
                Vector3d v1 = cameraFromStart - cameraToStart;

                var m1 = Matrix3d.CreateFromAxisAngle(Vector3d.Cross(v1, cameraUpStart), -(startPosY - pos.Y) / 180f / kk * (float)Math.PI);
                var m2 = Matrix3d.CreateFromAxisAngle(cameraUpStart, -(startPosX - pos.X) / 180f / kk * (float)Math.PI);                

                v1 *= m1;
                v1 *= m2;
                var up1 = cameraUpStart;

                Camera.Up = up1;                
                Camera.Eye = cameraToStart + v1;              
            }
        }

        public float AlongRotate = 0;
        public Camera Camera;

     

        public GLControl GLControl => (Control as GlControlGameControlWrapper).Control;
    
        public override void Attach(IGameControlWrapper control, Camera camera)
        {
            base.Attach(control, camera);
            Camera = camera;
            GLControl.MouseUp += Control_MouseUp1;
            //control.MouseDown += Control_MouseDown;

            GLControl.KeyUp += Control_KeyUp1;
            GLControl.KeyDown += Control_KeyDown1;
            GLControl.MouseWheel += Control_MouseWheel1;
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

        

        public override void Deattach(IGameControlWrapper control)
        {

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
            return GLControl.PointToClient(p);
        }

        public void MakeCurrent()
        {
           
            GLControl.MakeCurrent();
        }

        public int Width
        {
            get
            {
             
                return GLControl.Width;
            }
        }

        public Rectangle ClientRectangle
        {
            get
            {
                
                return GLControl.ClientRectangle;
            }
        }

        public void UpdateMatricies(Camera cam)
        {
            
            cam.UpdateMatricies(GLControl.Size);
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
                var shift = mr.Start - Camera.Eye;
                shift.Normalize();                
                if (delta > 0)
                {
                    camera.OrthoWidth /= 1.2f;                    
                    Camera cam2 = new Camera();
                    cam2.Eye = camera.Eye;
                    cam2.Target = camera.Target;
                    cam2.Up = camera.Up;
                    cam2.OrthoWidth = camera.OrthoWidth;
                    cam2.IsOrtho = camera.IsOrtho;

                    UpdateMatricies(cam2);
                    MouseRay mr2 = new MouseRay(cur.X, cur.Y, cam2);                    
                    var diff = mr.Start - mr2.Start;
                    shift *= diff.Length;
                    camera.Eye += shift;
                    camera.Target += shift;
                }
                else
                {
                    camera.OrthoWidth *= 1.2f;
                    
                    Camera cam2 = new Camera();
                    cam2.Eye = camera.Eye;
                    cam2.Target = camera.Target;
                    cam2.Up = camera.Up;
                    cam2.OrthoWidth = camera.OrthoWidth;
                    cam2.IsOrtho = camera.IsOrtho;

                    UpdateMatricies(cam2);
                    MouseRay mr2 = new MouseRay(cur.X, cur.Y, cam2);

                    var diff = mr.Start - mr2.Start;
                    shift *= diff.Length;
                    camera.Eye -= shift;
                    camera.Target -= shift;
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
                    camera.Eye += dir * zoomK;
                    camera.Target += dir * zoomK;
                }
                else
                {
                    camera.Eye -= dir * zoomK;
                    camera.Target -= dir * zoomK;
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
        
        public static Vector3d? lineIntersection(Vector3d planePoint, Vector3d planeNormal, Vector3d linePoint, Vector3d lineDirection)
        {
            if (Math.Abs(Vector3d.Dot(planeNormal, lineDirection)) < 10e-6f)
            {
                return null;
            }

            var dot1 = Vector3d.Dot(planeNormal, planePoint);
            var dot2 = Vector3d.Dot(planeNormal, linePoint);
            var dot3 = Vector3d.Dot(planeNormal, lineDirection);
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
            cameraFromStart = Camera.Eye;
            cameraToStart = Camera.Target;
            cameraUpStart = Camera.Up;

            if (e.IsMiddlePressed)
            {                
                var d1 = Camera.Eye - Camera.Target;
                //var plane1 : forw
                var crs1 = Vector3d.Cross(cameraUpStart, d1);                
                if (SnapModePlane)
                {
                    var inter = lineIntersection(Vector3.Zero, Vector3.UnitZ, Camera.Eye, Camera.Target - Camera.Eye);
                    if (inter != null)
                    {
                        drag = true;
                        Camera.Target = inter.Value;
                        cameraToStart = Camera.Target;
                    }
                }
                else if (SnapMode)
                {
                    var inter = lineIntersection(Camera.Target, crs1, Vector3.Zero, Vector3.UnitX);
                    if (inter != null)
                    {
                        drag = true;
                        Camera.Target = inter.Value;
                        cameraToStart = Camera.Target;
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
        Vector3d cameraFromStart;
        Vector3d cameraToStart;
        Vector3d cameraUpStart;
        public PointF CursorPosition
        {
            get
            {
                
                return GLControl.PointToClient(Cursor.Position);
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
