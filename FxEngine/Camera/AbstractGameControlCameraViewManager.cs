using FxEngine.Gui;
using FxEngine.Interfaces;
using OpenTK;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace FxEngine.Cameras
{
    public abstract class AbstractGameControlCameraViewManager : IGameControlCameraViewManager
    {
        public IGameControlWrapper Control { get; set; }
        public bool Enable { get; set; } = true;


        public void Attach(BaseGlDrawingContext ctx, Camera camera)
        {
            Attach(ctx.GameControl, camera);
        }

        public void Deattach(BaseGlDrawingContext ctx)
        {
            Deattach(ctx.GameControl);
        }

        public abstract void Deattach(IGameControlWrapper control);
        public abstract void Update();

        public virtual void Attach(IGameControlWrapper control, Camera camera)
        {
            Control = control;
        }

    }
}

