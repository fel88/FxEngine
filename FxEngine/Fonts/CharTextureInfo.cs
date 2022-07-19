using System.Drawing;

namespace FxEngine.Fonts
{
    public class CharTextureInfo
    {
        public CharTextureInfo(Atlas.CharInfo cc)
        {
            Char = cc;
        }
        public char Character
        {
            get
            {
                return (char)Char.Index;
            }
        }
        public Atlas.CharInfo Char;
        public int Texture;
        public int VertexBuffer;
        public Size Size;

    }
}
