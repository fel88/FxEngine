namespace FxEngine.Loaders.Collada
{
    public class Animation
    {
        private float length;//in seconds
        private KeyFrame[] keyFrames;


        public Animation(float lengthInSeconds, KeyFrame[] frames)
        {
            this.keyFrames = frames;
            this.length = lengthInSeconds;
        }


        public float getLength()
        {
            return length;
        }


        public KeyFrame[] getKeyFrames()
        {
            return keyFrames;
        }
    }
}


