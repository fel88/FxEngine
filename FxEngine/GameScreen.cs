using FxEngine;
using FxEngine.Gui;
using OpenTK.Input;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FxEngine
{
    public class GameScreen
    {
        public List<GlGuiElement> GuiElements = new List<GlGuiElement>();

        public virtual void Init(BaseGlDrawingContext ctx)
        {

        }

        public BaseGlDrawingContext gctx;
        //public BaseGlDrawingContext GetContext(Camera camera)
        //{
        //    return new GlDrawingContext() { TextRoutine = gctx.TextRoutine, GameWindow = gctx.GameWindow, Camera = camera };
        //}
      
        public virtual void Activate(BaseGlDrawingContext ctx)
        {
                        
        }

        public virtual void Deactivate(BaseGlDrawingContext ctx)
        {

        }

        public virtual void Draw(BaseGlDrawingContext ctx)
        {

        }

        public virtual void MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public virtual void MouseWheel(BaseGlDrawingContext ctx, MouseWheelEventArgs e)
        {

        }

        public virtual void UpdateKeys(BaseGlDrawingContext gctx)
        {

        }

        public virtual void KeyDown(BaseGlDrawingContext gctx, KeyboardKeyEventArgs e)
        {

        }

        public virtual void MouseUp(BaseGlDrawingContext gctx, MouseButtonEventArgs e)
        {
            
        }

        public virtual void LoadModels()
        {
            
        }

        public virtual void Load()
        {
            
        }
    }
}
