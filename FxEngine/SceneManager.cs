using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxEngine
{
    public class SceneManager
    {
        public Scene ActiveScene;

        public List<Scene> Scenes = new List<Scene>();
        public void Draw()
        {
            ActiveScene.Update();
            ActiveScene.Draw();
        }
    }
}
