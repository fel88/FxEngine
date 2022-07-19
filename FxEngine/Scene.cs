using FxEngine.Cameras;

namespace FxEngine
{
    public abstract class Scene
    {
        public Camera Camera;
        public abstract void Update();

        public abstract void Draw();
        
    }
}
