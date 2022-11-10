using FxEngine.Cameras;
using FxEngine.Loaders.Collada;

namespace FxEngine
{
    public class ColladaModelBlueprint : ModelBlueprint
    {
        public ColladaModelBlueprint(string nm)
            : base(nm)
        {

        }
        public ColladaModelBlueprint(string name, string path)
            : base(name, path)
        {

        }

        public ColladaModel Model;

        public override void Init()
        {
            if (Model != null)
            {
                Model.InitLibraries();
            }
        }

        public override void Draw(bool oldStyle, Camera camera, int shaderProgram)
        {
            if (oldStyle)
            {
                Model.DrawOldStyle();
            }
            else
            {
                Model.Draw(camera, shaderProgram);
            }
        }
    }
}
