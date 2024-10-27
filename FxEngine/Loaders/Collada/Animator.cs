using OpenTK;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace FxEngine.Loaders.Collada
{
    public class Animator
    {

        private ColladaJoint rootJoint;

        private Animation currentAnimation;
        private float animationTime = 0;




        /**
         * @param entity
         *            - the entity which will by animated by this animator.
         */
        public Animator(ColladaJoint rootJoint)
        {
            this.rootJoint = rootJoint;
        }


        public void doAnimation(Animation animation)
        {
            this.animationTime = 0;
            this.currentAnimation = animation;
        }

        public bool AllowTimeIncrease = true;

        public void update()
        {
            if (currentAnimation == null)
            {
                return;
            }

            Dictionary<String, Matrix4> currentPose = calculateCurrentAnimationPose();
            applyPoseToJoints(currentPose, rootJoint, Matrix4.Identity);
            if (AllowTimeIncrease)
            {
                increaseAnimationTime();
            }
        }


        private void increaseAnimationTime()
        {
            animationTime += DisplayManager.getFrameTime();
            if (animationTime > currentAnimation.getLength())
            {
                this.animationTime %= currentAnimation.getLength();
            }
        }

        private Dictionary<String, Matrix4> calculateCurrentAnimationPose()
        {
            KeyFrame[] frames = getPreviousAndNextFrames();
            float progression = calculateProgression(frames[0], frames[1]);
            return interpolatePoses(frames[0], frames[1], progression);
        }


        private void applyPoseToJoints(Dictionary<String, Matrix4> currentPose, ColladaJoint joint, Matrix4 parentTransform)
        {
            Matrix4 currentLocalTransform = currentPose[joint.Id];
            Matrix4 currentTransform = currentLocalTransform * parentTransform;
            foreach (ColladaJoint childJoint in joint.Childs)
            {
                applyPoseToJoints(currentPose, childJoint, currentTransform);
            }
            currentTransform = joint.InverseBindTransform * currentTransform;
            joint.AnimatedTransform = (currentTransform);
        }


        private KeyFrame[] getPreviousAndNextFrames()
        {
            KeyFrame[] allFrames = currentAnimation.getKeyFrames();
            KeyFrame previousFrame = allFrames[0];
            KeyFrame nextFrame = allFrames[0];
            for (int i = 1; i < allFrames.Length; i++)
            {
                nextFrame = allFrames[i];
                if (nextFrame.getTimeStamp() > animationTime)
                {
                    break;
                }
                previousFrame = allFrames[i];
            }
            return new KeyFrame[] { previousFrame, nextFrame };
        }


        private float calculateProgression(KeyFrame previousFrame, KeyFrame nextFrame)
        {
            float totalTime = nextFrame.getTimeStamp() - previousFrame.getTimeStamp();
            float currentTime = animationTime - previousFrame.getTimeStamp();
            return currentTime / totalTime;
        }


        private Dictionary<String, Matrix4> interpolatePoses(KeyFrame previousFrame, KeyFrame nextFrame, float progression)
        {
            Dictionary<String, Matrix4> currentPose = new Dictionary<string, Matrix4>();
            foreach (String jointName in previousFrame.getJointKeyFrames().Keys)
            {
                JointTransform previousTransform = previousFrame.getJointKeyFrames()[jointName];
                JointTransform nextTransform = nextFrame.getJointKeyFrames()[jointName];
                JointTransform currentTransform = JointTransform.interpolate(previousTransform, nextTransform, progression);
                currentPose.Add(jointName, currentTransform.getLocalTransform());
            }
            return currentPose;
        }

    }
}


