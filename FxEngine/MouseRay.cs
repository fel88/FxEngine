using FxEngine.Cameras;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxEngine
{
    public class MouseRay
    {
        public Vector3 _start;
        public Vector3 _end;

        public Vector3 Start { get { return _start; } }

        public Vector3 End { get { return _end; } }
        public MouseRay(float x, float y, Camera view)
        {
            int[] viewport = new int[4];
            Matrix4 modelMatrix, projMatrix;
            viewport = view.viewport;
            modelMatrix = view.ViewMatrix;
            projMatrix = view.ProjectionMatrix;


            _start = UnProject(new Vector3(x, y, 0.0f), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
            _end = UnProject(new Vector3(x, y, 1.0f), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
        }
        public MouseRay(Point mouse)
            : this(mouse.X, mouse.Y)
        {
        }

        public MouseRay(Vector3 start, Vector3 end)
        {
            _start = start;
            _end = end;

        }
        public static int[] viewport = new int[4];
        public static Matrix4 modelMatrix, projMatrix;

        public static Vector2 Project(Vector3 point, Matrix4 projection, Matrix4 view, Size viewport)
        {
            return Project(point, projection, view, Matrix4.Identity, viewport.Width, viewport.Height);            
        }

        public static bool WithinEpsilon(float a, float b)
        {
            float num = a - b;

            return (-1.401293E-45f <= num) && (num <= float.Epsilon);
        }
        public static Vector2 Project(Vector3 source, Matrix4 projection, Matrix4 view, Matrix4 world, int wid, int heig)
        {
            Vector3 ret = new Vector3();
            float MaxDepth = 10;
            float MinDepth = 1;
            float thisX = 0;
            float thisY = 0;
            Matrix4 matrix = OpenTK.Matrix4.Mult(Matrix4.Mult(world, view), projection);
            //Vector3 vector = Vector3.Transform(source, matrix);
            Vector4 vector = (new Vector4(source, 1) * matrix);
            float a = (((source.X * matrix.M14) + (source.Y * matrix.M24) + (source.Z * matrix.M34))) + matrix.M44;
            if (!WithinEpsilon(a, 1f))
            {
                vector = (Vector4)(vector / a);
            }
            vector.X = (((vector.X + 1f) * 0.5f) * wid) + thisX;
            vector.Y = (((-vector.Y + 1f) * 0.5f) * heig) + thisY;
            vector.Z = (vector.Z * (MaxDepth - MinDepth)) + MinDepth;
            return vector.Xy;

        }
        public static void UpdateMatrices()
        {
            GL.GetFloat(GetPName.ModelviewMatrix, out modelMatrix);
            GL.GetFloat(GetPName.ProjectionMatrix, out projMatrix);
            GL.GetInteger(GetPName.Viewport, viewport);
        }
        public static void UpdateMatrices(Camera cam)
        {
            modelMatrix = cam.ViewMatrix;
            projMatrix = cam.ProjectionMatrix;
            viewport = cam.viewport;
        }
        public Vector3 Dir
        {
            get { return (End - Start).Normalized(); }
        }

        public MouseRay(int x, int y, Camera view)
        {
            int[] viewport = new int[4];
            Matrix4 modelMatrix, projMatrix;
            viewport = view.viewport;
            modelMatrix = view.ViewMatrix;
            projMatrix = view.ProjectionMatrix;


            _start = UnProject(new Vector3(x, y, 0.0f), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
            _end = UnProject(new Vector3(x, y, 1.0f), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
        }

        public MouseRay(int x, int y)
        {

            _start = UnProject(new Vector3(x, y, 0.0f), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
            _end = UnProject(new Vector3(x, y, 1.0f), projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
        }

        public static Vector3 UnProject(Vector3 mouse, Matrix4 projection, Matrix4 view, Size viewport)
        {
            Vector4 vec;

            vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
            vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
            vec.Z = mouse.Z;
            vec.W = 1.0f;

            Matrix4 viewInv = Matrix4.Invert(view);
            Matrix4 projInv = Matrix4.Invert(projection);

            Vector4.Transform(ref vec, ref projInv, out vec);
            Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > 0.000001f || vec.W < -0.000001f)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return vec.Xyz;
        }

    }
}
