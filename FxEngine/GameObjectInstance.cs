using OpenTK;

namespace FxEngine
{
    public class GameObjectInstance
    {
        public GameObject GameObject;
        public int Id;
        public string Name;

        public Matrix4 Matrix { get; set; }
        public Vector3 Position { get; set; }
    }
}

