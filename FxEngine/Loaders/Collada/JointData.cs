using OpenTK;
using System;
using System.Collections.Generic;

namespace FxEngine.Loaders.Collada
{
    public class JointData
    {

        public int index;
        public String nameId;
        public Matrix4 bindLocalTransform;

        public List<JointData> children = new List<JointData>();

        public JointData(int index, String nameId, Matrix4 bindLocalTransform)
        {
            this.index = index;
            this.nameId = nameId;
            this.bindLocalTransform = bindLocalTransform;
        }

        public void addChild(JointData child)
        {
            children.Add(child);
        }

    }
}


