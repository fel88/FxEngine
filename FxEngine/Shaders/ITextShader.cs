namespace FxEngine.Shaders
{
    public interface ITextShader : IShader
    {
        void SetTexture();
        int AtlasTextureId { get; set; }
    }
}
