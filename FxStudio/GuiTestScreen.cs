using FxEngine;
using FxEngine.Gui;

namespace FxEngineEditor
{
    public class GuiTestScreen : GameScreen
    {
        public override void Draw(BaseGlDrawingContext ctx)
        {            
            foreach (var item in GuiElements)
            {
                item.Update(ctx);
                item.Draw(ctx);
            }
            base.Draw(ctx);
        }
    }
}
