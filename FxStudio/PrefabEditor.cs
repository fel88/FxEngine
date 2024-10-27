using FxEngine;
using FxEngine.Assets;
using FxEngine.Cameras;
using FxEngine.Fonts.SDF;
using FxEngine.Loaders.Collada;
using FxEngine.Loaders.OBJ;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FxEngineEditor
{
    public partial class PrefabEditor : Form
    {
        GLControl gl;
        MessageFilter mf = null;
        CameraViewManagerExt cvm = new CameraViewManagerExt();
        public PrefabEditor()
        {
            InitializeComponent();

            camera.CamFrom = new Vector3(-50, -50, 50);
            camera.CamTo = new Vector3(0, 0, 0);

            gl = new GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8));
            gl.Margin = new Padding(0);

            timer1.Interval = 10;
            timer1.Tick += new System.EventHandler(this.timer1_Tick);
            Controls.Add(gl);
            gl.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(gl, 0, 1);
            //this.MouseWheel += Form1_MouseWheel;

            gl.Resize += gl_Resize;
            gl.Paint += gl_Paint;
            gl.Load += gl_Load;

            UpdatePrefabsList();
            cvm.Attach(gl, camera);
            gl.MouseDown += cvm.Control_MouseDown1;

            mf = new MessageFilter();
            Application.AddMessageFilter(mf);
        }

        public void UpdatePrefabsList()
        {
            listView1.Items.Clear();
            if (Static.Library != null)
            {
                foreach (var item in Static.Library.Models)
                {
                    listView1.Items.Add(new ListViewItem(new string[] { item.Name }) { Tag = item });
                }
            }
        }

        float zoomK = 10;

        void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (
     gl.ClientRectangle.IntersectsWith(new Rectangle(gl.PointToClient(Cursor.Position),
                                                              new Size(1, 1))))
            {
                if (e.Delta > 0)
                {
                    var dir = camera.CamTo - camera.CamFrom;
                    dir.Normalize();
                    camera.CamFrom += dir * zoomK;
                }
                else
                {
                    var dir = camera.CamTo - camera.CamFrom;
                    dir.Normalize();
                    camera.CamFrom -= dir * zoomK;

                }
                gl.Invalidate();
            }
        }

        Timer timer1 = new Timer();

        void gl_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(Color.SkyBlue);
            tr.Init(CreateGraphics());
            glabel1 = new GlLabel();
            glabel1.Init();

            timer1.Enabled = true;
        }

        Camera camera = new Camera();
        public ModelBlueprint CurrentBlueprint = null;
        private float rotation = 0;
        void Render()
        {
            if (!loaded)
                return;
            if (Static.Library != null)
            {
                if (!Static.Library.Inited)
                {
                    Static.Library.Init();
                }
            }
            gl.MakeCurrent();
            cvm.Update();
            GL.ClearColor(Color.LightBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);

            camera.Setup(gl);

            if (gl.Focused)
                GL.Color3(Color.Yellow);
            else
            {

                GL.Color3(Color.Blue);
            }

            if (orthoView)
            {
                GL.Scale(zoomK, zoomK, zoomK);
            }

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.ShadeModel(ShadingModel.Smooth);
            float[] mat_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] mat_shininess = { 50.0f };
            float[] light_position = { 0.0f, 0.0f, 100.0f, 0.0f };
            float[] light_ambient = { 0.5f, 0.5f, 0.5f, 1.0f };

            GL.Material(MaterialFace.Front, MaterialParameter.Specular, mat_specular);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, mat_shininess);
            GL.Enable(EnableCap.ColorMaterial);
            //GL.LightModel(LightModelParameter.LightModelAmbient, light_ambient);
            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Position, light_position);
            GL.Light(LightName.Light0, LightParameter.Ambient, light_ambient);


            GL.PushMatrix();
            GL.Rotate(rotation, Vector3.UnitZ);

            if (CurrentBlueprint != null)
            {

                if (CurrentBlueprint is ColladaModelBlueprint cmb)
                {
                    var clm = cmb.Model;
                    if (clm.Scenes.First().Nodes.Any(z => z.Controller != null))
                    {
                        var cntrl = clm.Scenes.First().Nodes.First(z => z.Controller != null).Controller;
                        cntrl.UpdateBones();
                    }

                    var chl = clm.Scenes.First().Nodes.First().GetAllChilds(true);
                    foreach (var item in chl)
                    {
                        //item.Matrix = DrawInfo.GetMatrix();
                        item.Matrix = Matrix4.Identity;
                    }

                    clm.DrawOldStyle();
                }
                else if(CurrentBlueprint is ObjModelBlueprint omb)
                {
                    var bboxes = omb.Objs.Select(z => z.GetBoundingBox(Transform)).ToArray();
                    Vector3 mins = new Vector3(bboxes.Min(z => z.Position.X), bboxes.Min(z => z.Position.Y), bboxes.Min(z => z.Position.Z));
                    Vector3 maxs = new Vector3(bboxes.Max(z => z.Position.X + z.Size.X), bboxes.Max(z => z.Position.Y + z.Size.Y), bboxes.Max(z => z.Position.Z + z.Size.Z));
                    BoundingBox bbox = new BoundingBox() { Position = mins, Size = maxs - mins };
                    label2.Text = $"x:{bbox.Size.X} y:{bbox.Size.Y} z:{bbox.Size.Z}";
                    foreach (var v in omb.Objs)
                    {
                        var model = v;
                        if (model.GetVerts().Length == 0) 
                            continue;

                        var maxx = model.GetVerts().Max(x => x.X);
                        var minx = model.GetVerts().Min(x => x.X);
                        var maxy = model.GetVerts().Max(x => x.Y);
                        var miny = model.GetVerts().Min(x => x.Y);

                        GL.Color3(Color.White);

                        int indiceat = 0;
                        var t = v.faces;
                        foreach (var tuple in t)
                        {


                            var mater = tuple.Material;
                            if (mater != null)
                            {
                                GL.Color3(mater.DiffuseColor);
                                if (v.mat.textures.ContainsKey(mater.AmbientMap))
                                {


                                    GL.Enable(EnableCap.Texture2D);
                                    GL.BindTexture(TextureTarget.Texture2D, v.mat.textures[mater.AmbientMap]);

                                }
                                if (v.mat.textures.ContainsKey(mater.DiffuseMap))
                                {


                                    GL.Enable(EnableCap.Texture2D);

                                    GL.BindTexture(TextureTarget.Texture2D, v.mat.textures[mater.DiffuseMap]);

                                }
                            }


                            if (tuple.Vertexes.Count() == 3)
                            {
                                GL.Begin(PrimitiveType.Triangles);
                                GL.TexCoord2(tuple.Item1.TextureCoord);
                                GL.Normal3(tuple.Item1.Normal);
                                GL.Vertex3(tuple.Item1.Position * Transform);

                                GL.TexCoord2(tuple.Item2.TextureCoord);
                                GL.Normal3(tuple.Item2.Normal);
                                GL.Vertex3(tuple.Item2.Position * Transform);

                                GL.TexCoord2(tuple.Item3.TextureCoord);
                                GL.Normal3(tuple.Item3.Normal);
                                GL.Vertex3(tuple.Item3.Position * Transform);
                                GL.End();
                            }
                            if (tuple.Vertexes.Count() == 4)
                            {
                                GL.Begin(PrimitiveType.Quads);
                                GL.TexCoord2(tuple.Item1.TextureCoord);
                                GL.Normal3(tuple.Item1.Normal);
                                GL.Vertex3(tuple.Item1.Position * Transform);

                                GL.TexCoord2(tuple.Item2.TextureCoord);
                                GL.Normal3(tuple.Item2.Normal);
                                GL.Vertex3(tuple.Item2.Position * Transform);

                                GL.TexCoord2(tuple.Item3.TextureCoord);
                                GL.Normal3(tuple.Item3.Normal);
                                GL.Vertex3(tuple.Item3.Position * Transform);


                                GL.TexCoord2(tuple.Item4.TextureCoord);
                                GL.Normal3(tuple.Item4.Normal);
                                GL.Vertex3(tuple.Item4.Position * Transform);
                                GL.End();
                            }



                            GL.Disable(EnableCap.Texture2D);

                        }
                    }
                }


                /*
                GL.BindTexture(TextureTarget.Texture2D, v.TextureID);
                //GL.UniformMatrix4(shaders[activeShader].GetUniform("modelview"), false, ref v.ModelViewProjectionMatrix);

                if (shaders[activeShader].GetAttribute("maintexture") != -1)
                {
                    GL.Uniform1(shaders[activeShader].GetAttribute("maintexture"), v.TextureID);
                }

                GL.DrawElements(BeginMode.Triangles, v.IndiceCount, DrawElementsType.UnsignedInt, indiceat * sizeof(uint));
                indiceat += v.IndiceCount;*/

                GL.PopMatrix();
            }
            GL.PopMatrix();

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            //GL.Ortho(-r * aspect, r * aspect, -r, r, -100, 100);
            var o = Matrix4.CreateOrthographic(gl.Width, gl.Height, -1000, 10000);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.MultMatrix(ref o);

            var modelview = Matrix4.LookAt(new Vector3(0, 0, 1000), new Vector3(), Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            //DrawQuad(new PointF(), 1, 100, 100);
            glabel1.Position = new PointF(0
                , 0);
            //glabel1.Update();
            glabel1.Font = new Font("Arial", 18);
            glabel1.Text = "sfsdf";
            //glabel1.Draw();


            float tscl = 0.5f;
            tr.SetGamma(0.2f);
            GL.Translate(-gl.Width / 2, gl.Height / 2, 0);
            GL.Scale(tscl, tscl, tscl);
            var nm = "";
            if (CurrentBlueprint != null)
            {
                nm = CurrentBlueprint.Name;
            }

            tr.DrawText("name: " + nm, new PointF(0, 0));
            if (CurrentBlueprint != null)
            {
                if ((CurrentBlueprint is ObjModelBlueprint omb))
                {
                    GL.Translate(0, -60, 0);

                    var bboxes = omb.Objs.Select(z => z.GetBoundingBox(Transform)).ToArray();
                    Vector3 mins = new Vector3(bboxes.Min(z => z.Position.X), bboxes.Min(z => z.Position.Y), bboxes.Min(z => z.Position.Z));
                    Vector3 maxs = new Vector3(bboxes.Max(z => z.Position.X + z.Size.X), bboxes.Max(z => z.Position.Y + z.Size.Y), bboxes.Max(z => z.Position.Z + z.Size.Z));
                    BoundingBox bbox = new BoundingBox() { Position = mins, Size = maxs - mins };
                    tr.DrawText($"x:{bbox.Size.X} y:{bbox.Size.Y} z:{bbox.Size.Z}", new PointF(0, 0));
                }
            }
            gl.SwapBuffers();
        }

        GlLabel glabel1 = new GlLabel();
        public void DrawQuad(PointF position, float z = 1, float szw = 1, float szh = 1)
        {
            GL.PushMatrix();
            int[] vw = new int[4];
            GL.GetInteger(GetPName.Viewport, vw);
            //GL.Translate(-glControl.Width / 2, glControl.Height / 2 - 30, 0);
            GL.Translate(position.X, position.Y, 0);

            GL.Begin(PrimitiveType.Quads);
            var realWidth = szw;
            var realHeight = szh;

            GL.TexCoord3(1.0f, 1.0f, 0f); GL.Vertex3(realWidth, realHeight, z);
            GL.TexCoord3(0.0f, 1.0f, 0f); GL.Vertex3(0f, realHeight, z);
            GL.TexCoord3(0.0f, 0.0f, 0f); GL.Vertex3(0f, 0f, z);
            GL.TexCoord3(1.0f, 0.0f, 0f); GL.Vertex3(realWidth, 0f, z);
            /*GL.Vertex3(realWidth, realHeight, z);
            GL.Vertex3(0f, realHeight, z);
            GL.Vertex3(0f, 0f, z);
            GL.Vertex3(realWidth, 0f, z);*/
            GL.End();

            GL.PopMatrix();

        }
        SdfTextRoutine tr = new SdfTextRoutine();

        void gl_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
        bool orthoView = false;



        bool loaded = false;
        void gl_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            gl.Invalidate();
            if (checkBox2.Checked)
            {
                rotation += 0.5f;
            }
        }

        
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.FirstSelected() != null)
            {
                CurrentBlueprint = listView1.FirstSelected().Tag as ModelBlueprint;
                propertyGrid1.SelectedObject = CurrentBlueprint;
                UpdateObjsList();
            }
        }

        public void UpdateObjsList()
        {
            listView2.Items.Clear();
            if (CurrentBlueprint is ColladaModelBlueprint) { }
            else if(CurrentBlueprint is ObjModelBlueprint omb)
            {
                foreach (var item in omb.Objs)
                {
                    listView2.Items.Add(new ListViewItem(new string[] { item.Name + ".." }) { Tag = item });
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var f = float.Parse(textBox1.Text, CultureInfo.InvariantCulture);
                zoomK = f;
            }
            finally { }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            orthoView = checkBox1.Checked;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        Matrix3 Transform = Matrix3.Identity;
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var sc = float.Parse(textBox2.Text, CultureInfo.InvariantCulture);
                Transform *= Matrix3.CreateScale(sc, sc, sc);
            }
            catch (Exception ex)
            {

            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            fitAll();
        }

        void fitAll()
        {
            var vv = getAllPoints();
            if (vv.Length == 0) return;
            FitToPoints(vv, camera);
        }

        Vector3d[] getAllPoints()
        {
            var bbox = CurrentBlueprint.GetBoundingBoxModel(Matrix4.Identity);
            List<Vector3d> ret = new List<Vector3d>();
            foreach (var item in bbox)
            {
                ret.AddRange(item.Vertices.Select(z => z.Position.ToVector3d()));
            }
            return ret.ToArray();
        }

        public void FitToPoints(Vector3d[] pnts, Camera cam, float gap = 10)
        {
            List<Vector2d> vv = new List<Vector2d>();
            foreach (var vertex in pnts)
            {
                var p = MouseRay.Project(vertex.ToVector3(), cam.ProjectionMatrix, cam.ViewMatrix, new Size(cam.viewport[2], cam.viewport[3]));
                vv.Add(p.Xy.ToVector2d());
            }

            //prjs->xy coords
            var minx = vv.Min(z => z.X) - gap;
            var maxx = vv.Max(z => z.X) + gap;
            var miny = vv.Min(z => z.Y) - gap;
            var maxy = vv.Max(z => z.Y) + gap;

            var dx = (maxx - minx);
            var dy = (maxy - miny);

            var cx = dx / 2;
            var cy = dy / 2;
            var dir = cam.CamTo - cam.CamFrom;
            //center back to 3d

            var mr = new MouseRay((float)(cx + minx), (float)(cy + miny), cam);
            var v0 = mr.Start;

            cam.CamFrom = v0;
            cam.CamTo = cam.CamFrom + dir;

            var aspect = gl.Width / (float)(gl.Height);

            dx /= gl.Width;
            dx *= cam.OrthoWidth;
            dy /= gl.Height;
            dy *= cam.OrthoWidth;

            cam.OrthoWidth = (float)Math.Max(dx, dy);
        }
        
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            
        }

        private void objToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void objToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "OBJ models (*.obj)|*.obj";

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            var ll = (ObjVolume.LoadFromFile(ofd.FileName, Matrix4.Identity));
            var ff = new FileInfo(ofd.FileName);
            Static.Library.AddModel(new ObjModelBlueprint("obj export: " + ff.Name, ll) { Id = Static.Library.ModelNewId });

            UpdatePrefabsList();
        }

        private void colladaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Collada model (*.dae)|*.dae";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            var fi = new FileInfo(ofd.FileName);
            var clm = ColladaImporter.Load(ofd.FileName, new PhysicalFilesystemDataProvider());
            clm.InitLibraries();

            var mb = new ColladaModelBlueprint("collada export: " + fi.Name, fi.FullName) { Id = Static.Library.ModelNewId };
            mb.Model = clm;
            Static.Library.AddModel(mb);

            UpdatePrefabsList();

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Static.Library.AddModel(new ModelBlueprint("new model01"));
            UpdatePrefabsList();
        }

        private void exportToColladaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentBlueprint == null || !(CurrentBlueprint is ObjModelBlueprint omb)) 
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Static.Library.LibraryPath;
            sfd.DefaultExt = ".dae";
            sfd.Filter = "COLLADA files (*.dae)|*.dae";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ColladaRoutine.Export(omb, sfd.FileName);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            camera.CamFrom = new Vector3(-50, -50, 50);
            camera.CamTo = new Vector3(0, 0, 0);
        }
    }
}
