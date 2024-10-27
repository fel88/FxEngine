using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace FxEngine.Loaders.Collada
{
    public class AnimationLoader
    {

        /**
         * Loads up a collada animation file, and returns and animation created from
         * the extracted animation data from the file.
         * 
         * @param colladaFile
         *            - the collada file containing data about the desired
         *            animation.
         * @return The animation made from the data in the file.
         */
        public static Animation loadAnimation(string colladaFile, ColladaParseContext ctx)
        {
            AnimationData animationData = ColladaLoader.loadColladaAnimation(colladaFile, ctx);
            KeyFrame[] frames = new KeyFrame[animationData.keyFrames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = createKeyFrame(animationData.keyFrames[i]);
            }
            return new Animation(animationData.lengthSeconds, frames);
        }

        /**
         * Creates a keyframe from the data extracted from the collada file.
         * 
         * @param data
         *            - the data about the keyframe that was extracted from the
         *            collada file.
         * @return The keyframe.
         */
        private static KeyFrame createKeyFrame(KeyFrameData data)
        {
            Dictionary<String, JointTransform> map = new Dictionary<string, JointTransform>();
            foreach (JointTransformData jointData in data.jointTransforms)
            {
                JointTransform jointTransform = createTransform(jointData);
                map.Add(jointData.jointNameId, jointTransform);
            }
            return new KeyFrame(data.time, map);
        }

        /**
         * Creates a joint transform from the data extracted from the collada file.
         * 
         * @param data
         *            - the data from the collada file.
         * @return The joint transform.
         */
        private static JointTransform createTransform(JointTransformData data)
        {
            Matrix4 mat = data.jointLocalTransform;
            Vector3 translation = new Vector3(mat.M41, mat.M42, mat.M43);
            Quaternion rotation = Quaternion.FromMatrix(new Matrix3(mat));
            return new JointTransform(translation, rotation);
        }

    }
}


