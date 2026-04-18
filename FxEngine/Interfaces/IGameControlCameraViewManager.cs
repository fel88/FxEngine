using FxEngine.Cameras;

namespace FxEngine.Interfaces
{
    public interface IGameControlCameraViewManager
    {
        IGameControlWrapper Control { get; set; }
        bool Enable { get; set; }
        void Deattach(IGameControlWrapper control);
        void Attach(IGameControlWrapper control, Camera camera);
        void Update();

    }
}

