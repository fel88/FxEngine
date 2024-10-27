using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaInstanceController
    {

        public List<Vector3> TransformedVectors;

        public bool AllowAnimationTransform = false;
        public void UpdateNode(ColladaNode node)
        {
            UpdateAnimator();
            var jointTransforms = GetJointsTransform();



            var skin = Controller.Skin;
            var weights = skin.Weights;
            var joints = skin.Joints;

            var nm = skin.Source.VerticiesItem.Input.Source.Replace("#", "");
            var src0 = skin.Source.Sources.First(z => z.Id == nm);
            var nm0 = node.Id;
            var names = skin.Sources.First(z => z.Array is ColladaNameArray);
            var array = names.Array as ColladaNameArray;
            var index0 = Array.IndexOf(array.Names, nm0);
            var wsrcnm = weights.Inputs.Last().Source.Replace("#", "");
            var warr = skin.Sources.First(u => u.Id == wsrcnm);

            if (TransformedVectors != null)
            {
                TransformedVectors.Clear();
            }
            else
            {
                TransformedVectors = new List<Vector3>();
            }

            #region forward kinematics
            node.Matrix = node.OrigMatrix;
            if (node.Parent != null)
            {
                node.Matrix = node.Parent.Matrix * node.Matrix;
            }
            #endregion

            for (int index = 0; index < src0.Count; index++)
            {      /*  vec4 totalLocalPos = vec4(0.0);
              vec4 totalNormal = vec4(0.0);

              for (int i = 0; i < MAX_WEIGHTS; i++)
              {
                  mat4 jointTransform = jointTransforms[in_jointIndices[i]];
                  vec4 posePosition = jointTransform * vec4(in_position, 1.0);
                  totalLocalPos += posePosition * in_weights[i];

                  vec4 worldNormal = jointTransform * vec4(in_normal, 0.0);
                  totalNormal += worldNormal * in_weights[i];
              }

              gl_Position = projectionViewMatrix * totalLocalPos;*/

                Vector4 totalLocalPos = new Vector4();
                //get verticies of current node?
                var _v = (float[])src0.Accessor.GetElement(index);

                var v = new Vector4(_v[0], _v[1], _v[2], 1);
                var in_position = v;
                var cnt = weights.Vcount[index];
                Vector4 ret = new Vector4();
                for (int i = 0; i < cnt; i++)
                {
                    var joint = weights.Polygons[index].Items[i].Vals[0];
                    var nm1 = joints.Inputs.First().Source.Replace("#", "");
                    var src1 = skin.Sources.First(z => z.Id == nm1);
                    var nmar = src1.Array as ColladaNameArray;


                    var ind1 = Array.IndexOf(array.Names, joint);

                    var weight = weights.Polygons[index].Items[i].Vals[1];

                    var inv = joints.Inputs.Last().Source.Replace("#", "");
                    var src = skin.Sources.First(z => z.Id == inv);
                    var invmat = (Matrix4)src.Accessor.GetElement(index0);

                    var ww = ((float[])warr.Accessor.GetElement(weight))[0];
                    //Matrix4 jm = node.Matrix;
                    //var res = (((v * skin.BindShapeMatrix) * invmat) * jm) * ww;
                    //ret += res;

                    var in_weights = ww;

                    Matrix4 jointTransform = jointTransforms[array.Names[joint]];
                    Vector4 posePosition = in_position * jointTransform;
                    totalLocalPos += posePosition * in_weights;
                }

                //var reto = v * skin.BindShapeMatrix;
                //TransformedVectors.Add(reto.Xyz);
                if (AllowAnimationTransform)
                {
                    TransformedVectors.Add(totalLocalPos.Xyz);

                }
                else
                {
                    TransformedVectors.Add(in_position.Xyz);

                }
            }

            /*foreach (var item in node.Nodes)
            {
                UpdateNode(item);
            }*/
        }

        public void UpdateBones()
        {
            Skeleton.Matrix = Skeleton.OrigMatrix;
            UpdateNode(Skeleton);

        }
        public ColladaController Controller;
        public ColladaNode Skeleton;

        public ColladaJoint[] Joints;


        public Animator Animator;
        public Animation Animation;

        public void UpdateAnimator()
        {
            if (Animator == null)
            {
                Animator = new Animator(Skeleton.Joint);
                Skeleton.Joint.CalcInverseBindTransform(Matrix4.Identity);
                Animator.doAnimation(Animation);
            }
            DisplayManager.update();
            Animator.update();
        }

        public Dictionary<string, Matrix4> GetJointsTransform()
        {
            Joints = Skeleton.GetAllChilds(true).Where(z => z.Joint != null).Select(z => z.Joint).ToArray();
            Dictionary<string, Matrix4> jointMatrices = new Dictionary<string, Matrix4>();
            addJointsToArray(Skeleton.Joint, jointMatrices);
            return jointMatrices;
        }
        private void addJointsToArray(ColladaJoint headJoint, Dictionary<string, Matrix4> jointMatrices)
        {
            if (!jointMatrices.ContainsKey(headJoint.Id))
            {
                jointMatrices.Add(headJoint.Id, Matrix4.Identity);
            }
            jointMatrices[headJoint.Id] = headJoint.AnimatedTransform;
            foreach (var childJoint in headJoint.Childs)
            {
                addJointsToArray(childJoint, jointMatrices);
            }
        }

        public static ColladaInstanceController Parse(XElement fr, ColladaParseContext ctx)
        {
            ColladaInstanceController ret = new ColladaInstanceController();
            var url = fr.Attribute("url").Value.Replace("#", "");
            ret.Controller = ctx.Model.Libraries.OfType<ColladaControllersLibrary>().First().Controllers.First(z => z.Id == url);

            var f = fr.Element(XName.Get("skeleton", ctx.Ns)).Value.Replace("#", "");

            ret.Skeleton = ctx.CurrentScene.Nodes.FirstOrDefault(z => z.Id == f);
            if (ret.Skeleton == null)
            {
                foreach (var item in ctx.CurrentScene.Nodes)
                {
                    ret.Skeleton = item.GetAllChilds().FirstOrDefault(z => z.Id == f);
                    if (ret.Skeleton != null)
                    {
                        break;
                    }
                }
            }

            return ret;
        }
    }
}


