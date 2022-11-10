using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL;
using FxEngine;
using System.Windows.Forms;

namespace FxEngine.Loaders.OBJ
{
    public class MaterialStuff
    {
        public string FilePath;
        public void LoadMaterials(string dir, string filename, IDataProvider dp)
        {
            var pp = Path.Combine(dir, filename);
            FilePath = pp;
            foreach (var mat in Material.LoadFromFile(pp, dp))
            {
                if (!materials.ContainsKey(mat.Key))
                {
                    materials.Add(mat.Key, mat.Value);
                }
            }
            var d = new FileInfo(pp);
            var name = d.DirectoryName;

            // Load textures
            foreach (Material mat in materials.Values)
            {
                GetTDesc(name, mat.AmbientMap, dp);
                GetTDesc(name, mat.DiffuseMap, dp);
                GetTDesc(name, mat.SpecularMap, dp);
                GetTDesc(name, mat.NormalMap, dp);
                GetTDesc(name, mat.OpacityMap, dp);
            }

        }
        TextureDescriptor GetTDesc(string dir, string mapp, IDataProvider dp)
        {
            TextureDescriptor td = null;
            string amb1 = mapp;
            if (!Path.IsPathRooted(mapp))
            {
                amb1 = Path.Combine(dir, mapp);
            }
            
            var finf = new FileInfo(amb1);
            if (dp.IsFileExists(amb1) && !textures.ContainsKey(mapp))
            {
                td = loadImage(finf.FullName, dp);
                TDesc.Add(td);
                textures.Add(mapp, td.Index);
                textures2.Add(mapp, finf.FullName);
            }
            return td;
        }

        static int loadImage(Bitmap image, IDataProvider dp)
        {
            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                                             ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int lin1 = (int)TextureMinFilter.Linear;
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref lin1);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref lin1);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);


            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        static TextureDescriptor loadImage(string filename, IDataProvider dp)
        {
            try
            {
                //using (
                var file = dp.GetBitmap(filename);
                //Bitmap file = new Bitmap(filename);
                //)
                {
                    file.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    return new TextureDescriptor() { FilePath = filename, Index = loadImage(file, dp), Preview = file };
                }
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
        }

        public Dictionary<String, Material> materials = new Dictionary<string, Material>();
        //short path, full path, tindex
        public Dictionary<string, int> textures = new Dictionary<string, int>();
        public Dictionary<string, string> textures2 = new Dictionary<string, string>();
        public List<TextureDescriptor> TDesc = new List<TextureDescriptor>();
    }
}