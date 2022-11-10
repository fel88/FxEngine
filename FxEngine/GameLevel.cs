using FxEngine.Cameras;
using FxEngine.Shaders;
using FxEngine.Tiles;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace FxEngine
{
    public class GameLevel
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public int ModelNewId
        {
            get
            {
                if (Models.Any())
                {
                    return Models.Max(z => z.Id) + 1;
                }
                return 0;
            }
        }
        public int CameraNewId
        {
            get
            {
                if (Cameras.Any())
                {
                    return Cameras.Max(z => z.Id) + 1;
                }
                return 0;
            }
        }

        public List<GameObjectInstance> GameObjectInstances = new List<GameObjectInstance>();
        public List<ModelInstance> Models = new List<ModelInstance>();
        public List<TileDrawItem> Tiles = new List<TileDrawItem>();
        public List<Camera> Cameras = new List<Camera>();

        public void Draw(Camera camera, bool oldstyle = false)
        {
            GL.PushMatrix();            

            GL.Color3(Color.White);

            foreach (var item in Models)
            {
                GL.Color3(Color.White);
                //get coords here

                item.Draw(camera, oldstyle, ModelDrawShader.ShaderProgram);
            }

            GL.PopMatrix();
        }
        public void DrawTiles(Camera camera, bool oldstyle = false)
        {
            GL.PushMatrix();            

            GL.Color3(Color.White);


            foreach (var item in Tiles)
            {
                item.Tile.Position = item.Position;
                item.Tile.Scale = item.Scale;
                item.Tile.Draw();
            }
            GL.PopMatrix();
        }

        public void SaveToXml(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            StoreXml(sb);            
            File.WriteAllText(path, sb.ToString());
        }
        public void StoreXml(StringBuilder sb)
        {            
            sb.AppendLine($"<level id=\"{Id}\" name=\"{Name}\">");
            foreach (var item in Tiles)
            {
                sb.AppendLine($"<tile x=\"{item.Position.X}\" y=\"{item.Position.Y}\" z=\"{item.Z}\" tileId=\"{item.Tile.Id}\" scale=\"{item.Scale}\"/>");
            }
            foreach (var item in Models)
            {
                sb.AppendLine($"<modelInstance x=\"{item.Blueprint.Id}\" matrix=\"{item.Matrix.ToXml()}\"/>");
            }
            sb.AppendLine("</level>");         
        }
    }
}