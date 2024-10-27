using FxEngine;
using OpenTK.Mathematics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FxEngineEditor
{
    public class ColladaRoutine
    {
        public static void AppendTechniqueCommon(StringBuilder sb, string id, int count, int stride)
        {
            sb.AppendLine("<technique_common>");
            sb.AppendLine($"<accessor source=\"{id}\" count=\"{count}\" stride=\"{stride}\">");
            sb.AppendLine("<param name=\"X\" type=\"float\"/>");
            sb.AppendLine("<param name=\"Y\" type=\"float\"/>");
            sb.AppendLine("<param name=\"Z\" type=\"float\"/>");
            sb.AppendLine("</accessor>");
            sb.AppendLine("</technique_common>");
        }

        public static void AppendFloatArray(StringBuilder sb, string id, Vector3[] points)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";


            sb.AppendLine($"<float_array id=\"{id}\" count=\"{points.Count() * 3}\">");
            foreach (var item in points)
            {
                sb.AppendLine($"{item.X.ToString(nfi)} {item.Y.ToString(nfi)} {item.Z.ToString(nfi)}");
            }

            sb.AppendLine("</float_array>");
        }
        public static void AppendSource(StringBuilder sb, string id, Vector3[] points)
        {
            sb.AppendLine($"<source id=\"{id}\">");
            AppendFloatArray(sb, id + "-array", points);
            AppendTechniqueCommon(sb, $"#" + id + "-array", points.Count(), 3);
            sb.AppendLine("</source>");
        }

        public static void Export(ObjModelBlueprint model, string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<COLLADA xmlns = \"http://www.collada.org/2005/11/COLLADASchema\" version = \"1.5.0\" >");
            sb.AppendLine("<library_geometries>");

            foreach (var item in model.Objs)
            {
                sb.AppendLine($"<geometry id=\"{item.Name}\" name=\"{item.Name}\">");
                sb.AppendLine($"<mesh>");
                AppendSource(sb, $"{item.Name}-Pos", item.origVerts);
                AppendSource(sb, $"{item.Name}-Normal", item.origNorms);


                sb.AppendLine($"<vertices id=\"{item.Name}-Vtx\">");
                sb.AppendLine($"<input semantic=\"POSITION\" source=\"#{item.Name}-Pos\"/>");
                sb.AppendLine($"</vertices>");
                sb.AppendLine($"<triangles count=\"{item.faces.Count}\">");
                sb.AppendLine($"<input semantic=\"VERTEX\" source=\"#{item.Name}-Vtx\" offset=\"{0}\"/>");
                sb.AppendLine($"<input semantic=\"NORMAL\" source=\"#{item.Name}-0-Normal\" offset=\"{1}\"/>");
                foreach (var fitem in item.faces)
                {
                    sb.Append($"<p> ");
                    foreach (var fvitem in fitem.Vertexes)
                    {
                        sb.Append($"{fvitem.Temp.Vertex} ");
                        sb.Append($"{fvitem.Temp.Normal} ");
                    }
                    sb.AppendLine($"</p>");
                }
                sb.AppendLine($"</triangles>");

                sb.AppendLine($"</mesh>");
                sb.AppendLine("</geometry>");
            }

            sb.AppendLine("</library_geometries>");

            sb.AppendLine("<library_visual_scenes>");
            sb.AppendLine("<visual_scene>");
            sb.AppendLine("<node id=\"\" name=\"\">");
            sb.AppendLine("</node>");
            sb.AppendLine("</visual_scene>");

            sb.AppendLine("</library_visual_scenes>");
            sb.AppendLine("<scene>");
            sb.AppendLine("<instance_visual_scene url=\"#DefaultScene\"/>");
            sb.AppendLine("</scene>");
            sb.AppendLine("</COLLADA>");
            File.WriteAllText(path, sb.ToString());
        }
    }
}
