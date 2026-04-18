using FxEngine.Cameras;

namespace FxEngine.Interfaces
{
    public interface IGameControlCameraViewManager
    {
        IGameControlWrapper Control { get; set; }
        bool Enable { get; set; }
        void Detach(IGameControlWrapper control);
        void Attach(IGameControlWrapper control, Camera camera);
        void Update();

    }
}

