using OpenTK;
using System;
using System.Collections.Generic;

namespace FxEngine.Loaders.Collada
{
    public class ColladaJoint
    {
        public ColladaJoint(ColladaNode node)
        {
            Node = node;
        }

        public ColladaNode Node;
        public string Id
        {
            get
            {
                return Node.Id;
            }
            set
            {
                Node.Id = value;
            }
        }
        public string Name
        {
            get
            {
                return Node.Name;
            }
            set
            {
                Node.Name = value;
            }
        }

        public ColladaJoint(int index, String name, Matrix4 bindLocalTransform)
        {
            Id = index + "";
            Name = name;
            LocalBindTransform = bindLocalTransform;
        }

        Matrix4 animatedTransform;
        public Matrix4 AnimatedTransform
        {
            get
            {
                return animatedTransform;
            }
            set
            {
                animatedTransform = value;
            }
        }
        public Matrix4 LocalBindTransform;//JMi
        public Matrix4 InverseBindTransform;//IBMi
        public void CalcInverseBindTransform(Matrix4 parentBindTransform)
        {
            Matrix4 bindTransform = LocalBindTransform * parentBindTransform;
            InverseBindTransform = bindTransform.Inverted();
            //Matrix4.Invert(ref bindTransform, out InverseBindTransform);
            foreach (var child in Childs)
            {
                child.CalcInverseBindTransform(bindTransform);
            }
        }

        public ColladaJoint Parent;
        public List<ColladaJoint> Childs = new List<ColladaJoint>();
    }
}


