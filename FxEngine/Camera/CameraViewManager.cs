using FxEngine.Gui;
using OpenTK;

namespace FxEngine.Cameras
{
    public abstract class CameraViewManager
    {
        public object Control;
        public bool Enable = true;
        public OpenTK.Windowing.Desktop.GameWindow GameWindow
        {
            get
            {
                return Control as OpenTK.Windowing.Desktop.GameWindow;
            }
        }

        public OpenTK.GLControl.GLControl GlControl
        {
            get
            {
                return Control as OpenTK.GLControl.GLControl;
            }
        }

        public abstract void Deattach(OpenTK.Windowing.Desktop.GameWindow control);
        public abstract void Deattach(OpenTK.GLControl.GLControl control);
        public abstract void Attach(BaseGlDrawingContext ctx, Camera camera);
        public abstract void Deattach(BaseGlDrawingContext ctx);
        public abstract void Update();

        public virtual void Attach(OpenTK.Windowing.Desktop.GameWindow control, Camera camera)
        {
            Control = control;
        }

        public virtual void Attach(OpenTK.GLControl.GLControl control, Camera camera)
        {
            Control = control;
        }
    }
}

