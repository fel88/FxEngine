﻿using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using FxEngine.Loaders.OBJ;

namespace FxEngine.Shaders
{
    public class VaoModel
    {

        public List<VaoModelItem> Items = new List<VaoModelItem>();

        public int tcount;
        public int VAO, VBO;
        public void DrawVao(IShader shader)
        {
            int shaderProgram = shader.GetProgramId();
            GL.UseProgram(shaderProgram);
            shader.SetUniformsData();            

            int transformLoc = GL.GetUniformLocation(shaderProgram, "transform");
            Matrix4 matrix;
            GL.GetFloat(GetPName.ModelviewMatrix, out matrix);
            Matrix4 projm;
            GL.GetFloat(GetPName.ProjectionMatrix, out projm);

            Matrix4 m = matrix * projm;
            

            GL.UniformMatrix4(transformLoc, false, ref m);
            foreach (var item in Items)
            {
                item.Draw(shaderProgram);
            }
            GL.UseProgram(0);
            
        }
                
        public void ModelInit(ObjVolume[] vols)
        {   
            tcount = 0;
            Dictionary<Material, List<FaceItem2>> dic1 = new Dictionary<Material, List<FaceItem2>>();
            foreach (var objVolume in vols)
            {
                foreach (var objVolumeFace in objVolume.faces)
                {
                    if (objVolumeFace.Material == null) 
                        continue;

                    if (!dic1.ContainsKey(objVolumeFace.Material))
                    {
                        dic1.Add(objVolumeFace.Material, new List<FaceItem2>());
                    }
                    dic1[objVolumeFace.Material].Add(objVolumeFace);
                   

                }
            }
            foreach (var item in dic1.Keys)
            {
                Items.Add(new VaoModelItem(item, dic1[item].ToArray(), vols[0].mat));
            }
        }      
    }
}
