using OpenTK.Mathematics;

namespace FxEngine
{
    public class GameObjectInstance
    {
        public GameObject GameObject;
        public int Id;
        public string Name;

        public Matrix4d Matrix { get; set; }
        public Vector3d Position { get; set; }
    }
}

