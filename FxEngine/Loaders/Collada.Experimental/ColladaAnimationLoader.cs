using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaAnimationLoader
    {

        private static Matrix4 CORRECTION = Matrix4.CreateRotationX(-90 * (float)Math.PI / 180.0f);

        private XElement animationData;
        private XElement jointHierarchy;

        public ColladaAnimationLoader(XElement animationData, XElement jointHierarchy)
        {
            this.animationData = animationData;
            this.jointHierarchy = jointHierarchy;
        }

        public AnimationData extractAnimation(ColladaParseContext ctx)
        {
            String rootNode = findRootJointName(ctx);
            float[] times = getKeyTimes(ctx);
            float duration = times[times.Length - 1];
            KeyFrameData[] keyFrames = initKeyFrames(times);
            List<XElement> animationNodes = animationData.Elements(XName.Get("animation", ctx.Ns)).ToList();
            foreach (XElement jointNode in animationNodes)
            {
                loadJointTransforms(keyFrames, jointNode, rootNode, ctx);
            }
            return new AnimationData(duration, keyFrames);
        }

        private float[] getKeyTimes(ColladaParseContext ctx)
        {
            XElement timeData = animationData.Element(XName.Get("animation", ctx.Ns)).Element(ctx.GetXmlName("source")).Element(ctx.GetXmlName("float_array"));
            String[] rawTimes = timeData.Value.Split(new char[] { ' ' });
            float[] times = new float[rawTimes.Length];
            for (int i = 0; i < times.Length; i++)
            {
                times[i] = float.Parse(rawTimes[i], CultureInfo.InvariantCulture);
            }
            return times;
        }

        private KeyFrameData[] initKeyFrames(float[] times)
        {
            KeyFrameData[] frames = new KeyFrameData[times.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = new KeyFrameData(times[i]);
            }
            return frames;
        }

        private void loadJointTransforms(KeyFrameData[] frames, XElement jointData, String rootNodeId, ColladaParseContext ctx)
        {
            String jointNameId = getJointName(jointData, ctx);
            String dataId = getDataId(jointData, ctx);
            XElement transformData = jointData.Elements(ctx.GetXmlName("source")).First(z => z.Attribute("id").Value == dataId);
            String[] rawData = transformData.Element(ctx.GetXmlName("float_array")).Value.Split(new char[] { ' ' });
            processTransforms(jointNameId, rawData, frames, jointNameId == (rootNodeId));
        }

        private String getDataId(XElement jointData, ColladaParseContext ctx)
        {
            XElement node = jointData.Element(ctx.GetXmlName("sampler")).Elements(ctx.GetXmlName("input")).First(z => z.Attribute("semantic").Value == "OUTPUT");
            return node.Attribute("source").Value.Substring(1);
        }

        private String getJointName(XElement jointData, ColladaParseContext ctx)
        {
            XElement channelNode = jointData.Element(ctx.GetXmlName("channel"));
            String data = channelNode.Attribute("target").Value;
            return data.Split(new char[] { '/' })[0];
        }

        private void processTransforms(String jointName, String[] rawData, KeyFrameData[] keyFrames, bool root)
        {

            float[] matrixData = new float[16];
            for (int i = 0; i < keyFrames.Length; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    matrixData[j] = float.Parse(rawData[i * 16 + j], CultureInfo.InvariantCulture);
                }

                Matrix4 transform = new Matrix4(
                    new Vector4(matrixData[0], matrixData[1], matrixData[2], matrixData[3]),
                    new Vector4(matrixData[4], matrixData[5], matrixData[6], matrixData[7]),
                    new Vector4(matrixData[8], matrixData[9], matrixData[10], matrixData[11]),
                    new Vector4(matrixData[12], matrixData[13], matrixData[14], matrixData[15])
                    );

                transform.Transpose();
                if (root)
                {
                    //because up axis in Blender is different to up axis in game
                    //transform = CORRECTION * transform;
                }
                keyFrames[i].addJointTransform(new JointTransformData(jointName, transform));
            }
        }

        private String findRootJointName(ColladaParseContext ctx)
        {
            XElement skeleton = jointHierarchy.Element(XName.Get("visual_scene", ctx.Ns)).Elements(XName.Get("node", ctx.Ns)).First(z => z.Attribute("id").Value == "Armature");
            return skeleton.Element(XName.Get("node", ctx.Ns)).Attribute("id").Value;
        }

    }
}


