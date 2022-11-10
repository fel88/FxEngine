using OpenTK;
using OpenTK.Graphics.OpenGL;
using FxEngine.Cameras;

namespace FxEngine
{
    public class ModelInstance
    {
        public ModelInstance()
        {
            MatrixDriver.Changed = () =>
            {
                if (UseMatrixDriver)
                {
                    Matrix = MatrixDriver.GetMatrix();
                }
            };
        }
        public int Id;
        public bool Billboard { get; set; }
        public bool UseMatrixDriver { get; set; }
        public string Name;
        public ModelBlueprint Blueprint;
        public Matrix4 Matrix;
        public MatrixGenerator MatrixDriver = new MatrixGenerator();

        public void Draw(Camera camera, bool oldStyle = false, int shdp = -1)
        {
            //get coords here
            GL.PushMatrix();
            if (Billboard)
            {
                var bm = camera.GetBillboardMatrix(Matrix.ExtractTranslation());
                var scale = Matrix.ExtractScale();
                bm = Matrix4.CreateScale(scale) * bm;
                GL.MultMatrix(ref bm);


            }
            else
            {
                GL.MultMatrix(ref Matrix);
            }


            Blueprint.Draw(oldStyle, camera, shdp);
            

            GL.PopMatrix();
        }

        public ModelInstance Clone()
        {
            ModelInstance m = new FxEngine.ModelInstance();
            m.Blueprint = Blueprint;
            m.Id = Id;
            m.Name = Name;
            m.Matrix = Matrix;
            return m;
        }
    }
}

