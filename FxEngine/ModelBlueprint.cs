using FxEngine.Cameras;
using FxEngine.Loaders.OBJ;
using FxEngine.Shaders;
using OpenTK;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace FxEngine
{    
    public class ModelBlueprint
    {
        public ModelBlueprint()
        {

        }

        public DrawPolygon[] GetBoundingBoxModel(Matrix4 matrix)
        {

            List<DrawPolygon> dp = new List<DrawPolygon>();
            GetBbox();
            float[] mins = new float[3];
            float[] maxs = new float[3];
            List<FaceItem2> faces = new List<FaceItem2>();

            maxs = MaxsBbox;
            mins = MinsBbox;
            List<FaceVertex> vrt = new List<FaceVertex>();
            var v1 = new FaceVertex() { Position = new Vector3(mins[0], mins[1], mins[2]) };
            var v2 = new FaceVertex() { Position = new Vector3(maxs[0], mins[1], mins[2]) };
            var v3 = new FaceVertex() { Position = new Vector3(maxs[0], maxs[1], mins[2]) };
            var v4 = new FaceVertex() { Position = new Vector3(mins[0], maxs[1], mins[2]) };

            var v5 = new FaceVertex() { Position = new Vector3(mins[0], mins[1], maxs[2]) };
            var v6 = new FaceVertex() { Position = new Vector3(maxs[0], mins[1], maxs[2]) };
            var v7 = new FaceVertex() { Position = new Vector3(maxs[0], maxs[1], maxs[2]) };
            var v8 = new FaceVertex() { Position = new Vector3(mins[0], maxs[1], maxs[2]) };

            //bottom
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v1, v2, v3 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v1, v3, v4 } });
            //upper
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v5, v6, v7 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v5, v7, v8 } });
            //x-positive
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v2, v3, v6 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v3, v6, v7 } });
            //x-negative
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v1, v4, v5 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v4, v5, v8 } });
            //y-positive
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v4, v3, v7 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v4, v7, v8 } });
            //y-negative
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v1, v2, v6 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v1, v6, v5 } });

            foreach (var item in faces)
            {

                var d = new DrawPolygon();
                List<DrawVertex> dv = new List<DrawVertex>();
                foreach (var vitem in item.Vertexes)
                {

                    var trans1 = matrix.ExtractTranslation();
                    var v22 = Vector3.TransformVector(vitem.Position, matrix);
                    v22 += trans1;


                    dv.Add(new DrawVertex() { Position = v22 });
                }

                d.Vertices = dv.ToArray();
                dp.Add(d);


            }

            return dp.ToArray();
        }

        public int Id { get; set; }
        public string FilePath { get; set; }


        public string Name { get; set; }
        public ModelBlueprint(string name, string path)
        {
            FilePath = path;
            Name = name;
        }
        public ModelBlueprint(string name)
        {

            Name = name;
        }
        public Matrix4 Matrix;



        public VaoModel VaoModel;





        public bool BBoxDirty = true;
        public float[] MinsBbox;
        public float[] MaxsBbox;

        protected Matrix4 oldbbox = Matrix4.Identity;


        public virtual void Draw(bool oldStyle, Camera camera, int shaderProgram)
        {

        }

        public virtual void Init()
        {

        }
        public virtual Vector3 GetBbox(Matrix4? mtr = null)
        {
            return new Vector3();
        }
    }
}
