using OpenTK.Mathematics;

namespace FxEngine.Interfaces
{
    public interface ITextRenderer
    {
        void RenderText(string text, float x, float y);
        void RenderText(string text, Vector2d pos);
    }
}

