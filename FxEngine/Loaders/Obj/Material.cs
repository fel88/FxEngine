using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using OpenTK;
using FxEngine;

namespace FxEngine.Loaders.OBJ
{    
    public class Material
    {
        public string Name { get; set; }
        public Vector3 AmbientColor = new Vector3();
        public Vector3 DiffuseColor = new Vector3();
        public Vector3 SpecularColor = new Vector3();
        public float SpecularExponent = 1;
        public float Opacity = 1.0f;

        public string AmbientMap = "";
        public string DiffuseMap = "";
        public string SpecularMap = "";
        public string OpacityMap = "";
        public string NormalMap = "";

        public Material()
        {
        }

        public Material(Vector3 ambient, Vector3 diffuse, Vector3 specular, float specexponent = 1.0f, float opacity = 1.0f)
        {
            AmbientColor = ambient;
            DiffuseColor = diffuse;
            SpecularColor = specular;
            SpecularExponent = specexponent;
            Opacity = opacity;
        }



        public static Dictionary<string, Material> LoadFromFile(string filename,IDataProvider dp)
        {
            Dictionary<string, Material> mats = new Dictionary<string, Material>();            
            var currentmat = "";
            
            var mems = new MemoryStream(dp.GetFile(filename));            
            using (StreamReader reader = new StreamReader(mems))
            {
                string currentLine;

                while (!reader.EndOfStream)
                {
                    currentLine = reader.ReadLine();

                    if (!currentLine.StartsWith("newmtl"))
                    {
                        if (currentmat.StartsWith("newmtl"))
                        {
                            currentmat += currentLine + "\n";
                        }
                    }
                    else
                    {
                        if (currentmat.Length > 0)
                        {
                            Material newMat = new Material();
                            string newMatName = "";

                            newMat = LoadFromString(currentmat, out newMatName);
                            newMat.Name = newMatName;
                            mats.Add(newMatName, newMat);
                        }

                        currentmat = currentLine + "\n";
                    }
                }
            }
                        
            if (currentmat.Count((char c) => c == '\n') > 0)
            {
                Material newMat = new Material();
                var newMatName = "";

                newMat = LoadFromString(currentmat, out newMatName);
                newMat.Name = newMatName;
                mats.Add(newMatName, newMat);
            }
            /*}
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found: {0}", filename);
            }
            catch (Exception)
            {
                Console.WriteLine("Error loading file: {0}", filename);
            }*/

            return mats;
        }

        public static Material LoadFromString(string mat, out string name)
        {
            Material output = new Material();
            name = "";

            List<string> lines = mat.Split('\n').ToList();
                        
            lines = lines.SkipWhile(s => !s.StartsWith("newmtl ")).ToList();
                        
            if (lines.Count != 0)
            {                
                name = lines[0].Substring("newmtl ".Length);
            }
                        
            lines = lines.Select(s => s.Trim()).ToList();
                        
            foreach (var line in lines)
            {                
                if (line.Length < 3 || line.StartsWith("//") || line.StartsWith("#"))
                {
                    continue;
                }

                // ambient color
                if (line.StartsWith("Ka"))
                {
                    string[] colorparts = line.Substring(3).Split(' ');
                    
                    if (colorparts.Length < 3)
                    {
                        throw new ArgumentException("Invalid color data");
                    }

                    Vector3 vec = new Vector3();                    
                    vec.X = float.Parse(colorparts[0], CultureInfo.InvariantCulture);
                    vec.Y = float.Parse(colorparts[1], CultureInfo.InvariantCulture);
                    vec.Z = float.Parse(colorparts[2], CultureInfo.InvariantCulture);

                    output.AmbientColor = new Vector3(float.Parse(colorparts[0], CultureInfo.InvariantCulture), float.Parse(colorparts[1], CultureInfo.InvariantCulture), float.Parse(colorparts[2], CultureInfo.InvariantCulture));
                    
                }

                // diffuse color
                if (line.StartsWith("Kd"))
                {
                    string[] colorparts = line.Substring(3).Split(' ');
                    
                    if (colorparts.Length < 3)
                    {
                        throw new ArgumentException("Invalid color data");
                    }

                    Vector3 vec = new Vector3();

                    // Attempt to parse each part of the color
                    vec.X = float.Parse(colorparts[0], CultureInfo.InvariantCulture);
                    vec.Y = float.Parse(colorparts[1], CultureInfo.InvariantCulture);
                    vec.Z = float.Parse(colorparts[2], CultureInfo.InvariantCulture);

                    output.DiffuseColor = new Vector3(float.Parse(colorparts[0], CultureInfo.InvariantCulture), float.Parse(colorparts[1], CultureInfo.InvariantCulture), float.Parse(colorparts[2], CultureInfo.InvariantCulture));                    
                }

                // specular color
                if (line.StartsWith("Ks"))
                {
                    string[] colorparts = line.Substring(3).Split(' ');
                                        
                    if (colorparts.Length < 3)
                    {
                        throw new ArgumentException("Invalid color data");
                    }

                    Vector3 vec = new Vector3();
                    
                    vec.X = float.Parse(colorparts[0], CultureInfo.InvariantCulture);
                    vec.Y = float.Parse(colorparts[1], CultureInfo.InvariantCulture);
                    vec.Z = float.Parse(colorparts[2], CultureInfo.InvariantCulture);

                    output.SpecularColor = new Vector3(float.Parse(colorparts[0], CultureInfo.InvariantCulture), float.Parse(colorparts[1], CultureInfo.InvariantCulture), float.Parse(colorparts[2], CultureInfo.InvariantCulture));
                }

                // specular exponent
                if (line.StartsWith("Ns"))
                {                    
                    float exponent = 0.0f;
                    bool success = float.TryParse(line.Substring(3), NumberStyles.Float, CultureInfo.InvariantCulture, out exponent);

                    output.SpecularExponent = exponent;
                                        
                    if (!success)
                    {
                        Console.WriteLine("Error parsing specular exponent: {0}", line);
                    }
                }

                // ambient map
                if (line.StartsWith("map_Ka"))
                {                    
                    if (line.Length > "map_Ka".Length + 6)
                    {
                        output.AmbientMap = line.Substring("map_Ka".Length + 1);
                    }
                }

                // diffuse map
                if (line.StartsWith("map_Kd"))
                {                    
                    if (line.Length > "map_Kd".Length + 4)
                    {
                        output.DiffuseMap = line.Substring("map_Kd".Length + 1);
                    }
                }

                // specular map
                if (line.StartsWith("map_Ks"))
                {                    
                    if (line.Length > "map_Ks".Length + 6)
                    {
                        output.SpecularMap = line.Substring("map_Ks".Length + 1);
                    }
                }

                // normal map
                if (line.StartsWith("map_normal"))
                {                    
                    if (line.Length > "map_normal".Length + 6)
                    {
                        output.NormalMap = line.Substring("map_normal".Length + 1);
                    }
                }

                // opacity map
                if (line.StartsWith("map_opacity"))
                {                    
                    if (line.Length > "map_opacity".Length + 6)
                    {
                        output.OpacityMap = line.Substring("map_opacity".Length + 1);
                    }
                }
            }
            return output;
        }
    }
}