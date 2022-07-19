using FxEngine.Gui;
using OpenTK;

namespace FxEngine.Cameras
{
    public abstract class CameraViewManager
    {
        public object Control;
        public bool Enable = true;
        public GameWindow GameWindow
        {
            get
            {
                return Control as GameWindow;
            }
        }

        public GLControl GlControl
        {
            get
            {
                return Control as GLControl;
            }
        }

        public abstract void Deattach(GameWindow control);
        public abstract void Deattach(GLControl control);
        public abstract void Attach(BaseGlDrawingContext ctx, Camera camera);
        public abstract void Deattach(BaseGlDrawingContext ctx);
        public abstract void Update();

        public virtual void Attach(GameWindow control, Camera camera)
        {
            Control = control;
        }

        public virtual void Attach(GLControl control, Camera camera)
        {
            Control = control;
        }
    }
}

