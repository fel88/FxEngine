namespace FxEngine.Shaders
{
    public interface IShader
    {
        int GetProgramId();
        void SetUniformsData();
        void Init();
        void Use();
    }
}
