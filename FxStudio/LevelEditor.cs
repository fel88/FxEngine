using FxEngine;
using FxEngine.Cameras;
using FxEngine.Fonts.SDF;
using FxEngine.Shaders;
using FxEngine.Tiles;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace FxEngineEditor
{
    public partial class LevelEditor : Form
    {
        OpenTK.GLControl.GLControl gl;
     
        public LevelEditor()
        {

            InitializeComponent();

            propertyGrid6.SelectedObject = new Vector4Wrapper();
            propertyGrid5.SelectedObject = ls;
            camera.CamFrom = new Vector3(170, 170, 170);
            UpdateTilesList();
            UpdateModelsList();
            UpdateLevels();
            OpenTK.GLControl.GLControlSettings settings = new OpenTK.GLControl.GLControlSettings();
            settings.NumberOfSamples = 8;
            settings.StencilBits = 0;
            settings.DepthBits = 24;
            settings.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            gl = new OpenTK.GLControl.GLControl(settings);

            //gl = new OpenTK.GLControl.GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8));
            // 
            gl.Margin = new Padding(0);

            Application.Idle += Application_Idle;
            gl.KeyDown += Gl_KeyDown;
            Controls.Add(gl);
            gl.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(gl, 0, 1);
            this.MouseWheel += Form1_MouseWheel;
            gl.MouseUp += Gl_MouseUp;
            gl.Resize += gl_Resize;
            gl.Paint += gl_Paint;
            gl.Load += gl_Load;
            gl.MouseDown += Gl_MouseDown;
            gl.MouseEnter += Gl_MouseEnter;
            gl.KeyUp += Gl_KeyUp;
            gl.MouseDoubleClick += Gl_MouseDoubleClick;
            Application.Idle += Application_Idle1;

        }

        private void Gl_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                BrushTile = null;
                BrushModel = null;
            }
        }

        private void Gl_KeyUp(object sender, KeyEventArgs e)
        {
            int code = (int)e.KeyCode;
            if (code >= 0 && code <= 255)
            {
                keys[code] = false;
            }
        }

        private void Gl_MouseEnter(object sender, EventArgs e)
        {
            gl.Focus();
        }

        private void Gl_KeyDown(object sender, KeyEventArgs e)
        {
            int code = (int)e.KeyCode;
            if (code >= 0 && code <= 255)
            {
                keys[code] = true;
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, System.Windows.Forms.Keys keyData)
        {
          
            int code = (int)keyData;
            if (code >= 0 && code <= 255)
            {
                keys[code] = true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        public void UpdateLevels()
        {
            listView2.Items.Clear();
            foreach (var item in Static.Library.Levels)
            {
                listView2.Items.Add(new ListViewItem(new string[] { item.Id + "", item.Name }) { Tag = item });
            }
        }

        float startPosX;
        float startPosY;
        Vector3 cameraFromStart;
        Vector3 cameraToStart;
        private void Gl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var pos = CursorPosition;
            startPosX = pos.X;
            startPosY = pos.Y;
            cameraFromStart = camera.CamFrom;
            cameraToStart = camera.CamTo;

            if (e.Button == MouseButtons.Left)
            {
                pressed = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                drag = true;
            }
            if (e.Button == MouseButtons.Middle)
            {

                if (BrushModel != null)
                {
                    dragm = true;
                    startScale = BrushModel.MatrixDriver.Scale;
                }
            }
        }

        private void Application_Idle1(object sender, EventArgs e)
        {
            gl.Invalidate();
        }

        public void UpdateTilesList()
        {
            listView3.Items.Clear();
            foreach (var item in Static.Library.Tiles)
            {
                listView3.Items.Add(new ListViewItem(new string[] { item.Id + "", item.Name }) { Tag = item });
            }
        }

        public void UpdateModelsList()
        {
            listView1.Items.Clear();
            foreach (var item in Static.Library.Models)
            {
                listView1.Items.Add(new ListViewItem(new string[] { item.Id + "", item.Name }) { Tag = item });
            }
            listView4.Items.Clear();
            if (Level != null)
            {
                foreach (var item in Level.Models)
                {
                    listView4.Items.Add(new ListViewItem(new string[] { item.Id + "", item.Name, item.Blueprint.Name }) { Tag = item });
                }
            }
        }

        public void UpdateCamerasList()
        {
            listView5.Items.Clear();
            foreach (var item in Level.Cameras)
            {
                listView5.Items.Add(new ListViewItem(new string[] { item.Id + "", item.Name }) { Tag = item });
            }

        }

        public Tile BrushTile = null;
        public ModelInstance BrushModel = null;
        public PointF GetPosition(bool gridSnap = false)
        {
            if (gridSnap)
            {
                return GetPositionGridSnap();
            }
        
            var v = System.Windows.Forms.Cursor.Position;
            var cp = gl.PointToClient(v);

            var f = GetFloorPosition();
            return new PointF(f.X, f.Y);
        }

        public PointF GetPositionGridSnap()
        {
            var pos = GetRawPositionGridSnap();
            return new PointF(pos.X * BrushTile.bmpt.Width, pos.Y * BrushTile.bmpt.Height);
        }

        public Point GetRawPositionGridSnap()
        {
            var cp = gl.PointToClient(System.Windows.Forms.Cursor.Position);
            float cpx = cp.X - gl.Width / 2;
            float cpy = -cp.Y + gl.Height / 2;
            cpx += (int)camera.CamTo.X;
            cpy += (int)camera.CamTo.Y;

            var f = GetFloorPosition();
            cpx = f.X;
            cpy = f.Y;

            return new Point((int)Math.Round(((cpx) / BrushTile.bmpt.Width)), (int)Math.Round((cpy) / BrushTile.bmpt.Height));
        }

        public void PlaceTile()
        {
            PlaceTile(GetPosition(checkBox1.Checked));
        }
        public void PlaceTile(PointF pos)
        {
            if (BrushTile != null && Level != null)
            {

                if (checkBox2.Checked)
                {
                    if (Level.Tiles.Any(z => z.Position == pos))
                    {
                        Level.Tiles.RemoveAll(z => z.Position == pos);
                    }
                    Level.Tiles.Add(new TileDrawItem() { Tile = BrushTile, Position = pos });
                    Static.Library.Dirty = true;
                }
                else
                {
                    if (!Level.Tiles.Any(z => z.Position == pos))
                    {
                        Level.Tiles.Add(new TileDrawItem() { Tile = BrushTile, Position = pos });
                    }
                }
            }
        }
        bool pressed = false;
        private void Gl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            drag = false;
            dragm = false;
            pressed = false;
            if (e.Button == MouseButtons.Left)
            {
                if (BrushModel != null && Level != null)
                {
                    Level.Models.Add(new ModelInstance() { Blueprint = BrushModel.Blueprint, Matrix = Matrix4.Identity, Name = BrushModel.Name + " inst", Id = Level.ModelNewId });
                    Level.Models.Last().UseMatrixDriver = true;
                    Level.Models.Last().MatrixDriver.position = BrushModel.MatrixDriver.position;
                    Level.Models.Last().MatrixDriver.Scale = BrushModel.MatrixDriver.Scale;
                    Static.Library.Dirty = true;
                    UpdateModelsList();
                }
            }
        }

        Camera camera = new Camera();
        void Form1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            
            float zoomK = 100;
            if (
     gl.ClientRectangle.IntersectsWith(new Rectangle(gl.PointToClient(System.Windows.Forms.Cursor.Position),
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
            timer1.Enabled = true;
            tr.Init(CreateGraphics());
        }


        private Matrix4 lookat;


        private float rotation = 0;


        bool[] keys = new bool[256];
        public bool IsKeyPressed(Keys k)
        {
            return keys[(int)k];
        }

        public PointF CursorPosition
        {
            get
            {
                return gl.PointToClient(System.Windows.Forms.Cursor.Position);
            }
        }
        bool drag = false;
        bool dragm = false;

        public Vector2 GetFloorPosition()
        {
            var curp = System.Windows.Forms.Cursor.Position;

            MouseRay mr = new MouseRay(gl.PointToClient(curp));
            var nearPoint3D = mr.Start;
            var dir1 = mr.Dir;
            //find intersection with plane
            bool was = false;

            float t = -nearPoint3D.Z / dir1.Z;
            Vector3 inter = nearPoint3D + dir1 * t;

            return inter.Xy;
        }

        float startScale;
        public void UpdateKeys()
        {

            var dir = camera.CamFrom - camera.CamTo;
            var cv = dir;
            var moveVec = new Vector3(cv.X, cv.Y, 0).Normalized();
            var moveVecTan = new Vector3(-moveVec.Y, moveVec.X, 0);
            var pos = CursorPosition;
            float zoom = 1;
            if (drag)
            {
                var dx = moveVecTan * ((startPosX - pos.X) / zoom) + moveVec * ((startPosY - pos.Y) / zoom);
                camera.CamFrom = cameraFromStart + dx;
                camera.CamTo = cameraToStart + dx;

            }
            if (dragm)
            {
                if (BrushModel != null)
                {
                    BrushModel.MatrixDriver.Scale = startScale + (startPosY - pos.Y);
                }
            }

            if (IsKeyPressed(Keys.PageDown))
            {
                camera.CamFrom += camera.CamUp * 10;

            }
            if (IsKeyPressed(Keys.PageUp))
            {
                camera.CamFrom -= camera.CamUp * 10;

            }
            if (IsKeyPressed(Keys.Home))
            {
                var dir2 = camera.CamFrom - camera.CamTo;
                dir2 = dir2 * Matrix3.CreateRotationZ(0.03f);
                camera.CamFrom = camera.CamTo + dir2;
            }

            if (IsKeyPressed(Keys.End))
            {
                var dir2 = camera.CamFrom - camera.CamTo;
                dir2 = dir2 * Matrix3.CreateRotationZ(-0.03f);
                camera.CamFrom = camera.CamTo + dir2;
            }

        }
        public static Shader ModelShader = new ModelDrawShader("model.vs", "model.fs");

        LightSource ls = new LightSource();
        bool firstInited = true;

        ModelInstance actor;
        void Render()
        {
            if (firstInited)
            {
                firstInited = false;
                ModelShader.Init();
            }
            
            /*var state = GamePad.GetState(0);
            listBox1.Items.Clear();
            listBox1.Items.Add(state.Buttons.A);
            listBox1.Items.Add(state.Buttons.B);
            listBox1.Items.Add(state.Buttons.X);
            listBox1.Items.Add(state.ThumbSticks.Left);*/
            if (Level != null && checkBox3.Checked)
            {
                //var p = Level.Models.First(z => z.Blueprint.Name.Contains("actor"));
                var p = actor;
                p.UseMatrixDriver = true;
                p.MatrixDriver.Scale = 20;
                p.MatrixDriver.PositionZ = 80;
                /*var vec = state.ThumbSticks.Left;
                if (vec.Length > 0.1)
                {
                    var atan2 = Math.Atan2(vec.Y, vec.X);
                    p.MatrixDriver.RotationZ = (float)atan2 * 180.0f / (float)Math.PI;

                    p.MatrixDriver.position -= 6 * new Vector3(vec.X, vec.Y, 0);
                }*/
            }


            if (!loaded)
                return;

            UpdateKeys();
            if (pressed && BrushTile != null)
            {
                int bsize = (int)numericUpDown1.Value;
                var pp = GetPosition(checkBox1.Checked);
                for (int i = -bsize / 2; i < (bsize / 2) + 1; i++)
                {
                    for (int j = -bsize / 2; j < (bsize / 2) + 1; j++)
                    {
                        PlaceTile(new PointF(pp.X + i * BrushTile.bmpt.Width, pp.Y + j * BrushTile.bmpt.Height));
                    }
                }
            }

            gl.MakeCurrent();
            if (!Static.Library.Inited)
            {
                Static.Library.Init();
            }

            GL.ClearColor(Color.LightBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            camera.Setup(gl);
            MouseRay.UpdateMatrices();

            GL.Enable(EnableCap.Lighting);

            GL.ShadeModel(ShadingModel.Smooth);
            float[] mat_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] mat_shininess = { 50.0f };

            GL.Enable(EnableCap.ColorMaterial);

            GL.PushMatrix();
            GL.Rotate(rotation, Vector3.UnitZ);
            ls.Setup(0);

            DrawAxis();

            GL.LineWidth(1);

            if (BrushTile != null)
            {
                int bsize = (int)numericUpDown1.Value;
                var pp = GetPosition(checkBox1.Checked);
                for (int i = -bsize / 2; i < (bsize / 2) + 1; i++)
                {
                    for (int j = -bsize / 2; j < (bsize / 2) + 1; j++)
                    {
                        //get coords here
                        BrushTile.Wireframe = true;
                        BrushTile.Position = new PointF(pp.X + i * BrushTile.bmpt.Width, pp.Y + j * BrushTile.bmpt.Height);
                        BrushTile.Draw();
                        BrushTile.Wireframe = false;
                    }
                }
            }

            GL.Enable(EnableCap.RescaleNormal);
            GL.Enable(EnableCap.Normalize);
            if (BrushModel != null)
            {
                var pp = GetPosition();
                //get coords here
                GL.PushMatrix();

                BrushModel.MatrixDriver.position = new Vector3(pp.X, pp.Y, 0);
                BrushModel.MatrixDriver.OnChanged();

                GL.MultMatrix(ref BrushModel.Matrix);

                BrushModel.Blueprint.Draw(true, null, -1);
                GL.PopMatrix();
                //GL.Color3(Color.White);
            }


            if (Level != null)
            {
                Level.Draw(camera, oldStyleDraw);
                Level.DrawTiles(camera);
            }
            GL.PopMatrix();
            DrawHud();
            gl.SwapBuffers();
        }

        private void DrawAxis()
        {
            GL.LineWidth(2);
            GL.Color3(Color.Red);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(100, 0, 0);
            GL.End();
            GL.LineWidth(2);
            GL.Color3(Color.Green);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 100, 0);
            GL.End();
            GL.Color3(Color.Blue);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 100);
            GL.End();
        }

        public void DrawHud()
        {
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

            float tscl = 0.5f;

            tr.SetGamma(0.2f);
            GL.Translate(-gl.Width / 2, gl.Height / 2, 0);
            GL.Scale(tscl, tscl, tscl);

            if (Level != null)
            {

                tr.DrawText($"floor count: {Level.Tiles.Count};  models: {Level.Models.Count}", new PointF(0, 0));
            }

            if (checkBox1.Checked && BrushTile != null)
            {
                tr.DrawText("snap position: " + GetRawPositionGridSnap(), new PointF(0, -40));
            }
            tr.DrawText("ds: " + (dt), new PointF(0, -80));

            GL.Disable(EnableCap.Blend);
        }

        float dt = 100;
        SdfTextRoutine tr = new SdfTextRoutine();

        void gl_Paint(object sender, PaintEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Render();

            sw.Stop();
            dt = sw.ElapsedMilliseconds;
        }

        bool loaded = false;
        void gl_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;
        }

        private void Application_Idle(object sender, EventArgs e)
        {

        }


        public GameLevel Level;
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Level = new GameLevel();
            Static.Library.AddLevel(Level);
            if (Static.Library.Levels.Any())
            {
                Level.Id = Static.Library.Levels.Max(z => z.Id) + 1;
            }
            UpdateLevels();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count > 0)
            {
                var tt = listView3.SelectedItems[0].Tag as Tile;
                pictureBox1.Image = tt.bmpt;
                BrushTile = tt;
                BrushModel = null;
            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            camera.CamFrom = new Vector3(0, 0, 100);
            camera.CamUp = Vector3.UnitY;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            camera.CamFrom = new Vector3(100, 100, 100);
            camera.CamUp = Vector3.UnitZ;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            BrushTile = null;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (BrushTile == null || Level == null) return;
            for (int i = -20; i < 20; i++)
            {
                for (int j = -20; j < 20; j++)
                {
                    Level.Tiles.Add(new TileDrawItem() { Tile = BrushTile, Position = new PointF(i * BrushTile.bmpt.Width, j * BrushTile.bmpt.Height) });
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (Level != null)
            {
                Level.Tiles.Clear();
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                var l = listView2.SelectedItems[0].Tag as GameLevel;
                Level = l;
                propertyGrid1.SelectedObject = l;
                UpdateCamerasList();
                UpdateModelsList();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var tt = listView1.SelectedItems[0].Tag as ModelBlueprint;
                BrushModel = new ModelInstance() { Blueprint = tt };
                BrushModel.UseMatrixDriver = true;
                //BrushModel.Matrix = Matrix4.CreateScale(10);
                BrushModel.Matrix = Matrix4.Identity;

                BrushTile = null;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Level.Models.Clear();
            UpdateModelsList();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count > 0)
            {
                var m = listView4.SelectedItems[0].Tag as ModelInstance;
                Level.Models.Remove(m);
                Static.Library.Dirty = true;
                UpdateModelsList();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count > 0)
            {
                var m = listView4.SelectedItems[0].Tag as ModelInstance;
                var fl = float.Parse(textBox2.Text, CultureInfo.InvariantCulture);
                m.Matrix = Matrix4.CreateScale(fl) * m.Matrix;
            }
        }

        private void listView4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count > 0)
            {
                var m = listView4.SelectedItems[0].Tag as ModelInstance;
                propertyGrid2.SelectedObject = m;
                propertyGrid3.SelectedObject = m.MatrixDriver;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Level.Cameras.Add(new Camera() { CamFrom = camera.CamFrom, CamTo = camera.CamTo, CamUp = camera.CamUp, IsOrtho = camera.IsOrtho, Id = Level.CameraNewId });
            Static.Library.Dirty = true;
            UpdateCamerasList();

        }

        private void switchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count > 0)
            {
                var cam = listView5.SelectedItems[0].Tag as Camera;

                camera = cam;
            }
        }

        private void listView5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count > 0)
            {
                var cam = listView5.SelectedItems[0].Tag as Camera;
                properyListView1.SetObject(cam);

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var v = propertyGrid6.SelectedObject as Vector4Wrapper;
            var ls = propertyGrid5.SelectedObject as LightSource;
            ls.Position = v.Vector;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void propertyGrid3_Click(object sender, EventArgs e)
        {

        }

        private void propertyGrid3_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            Static.Library.Dirty = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Level.Cameras.Add(new Camera() { Id = Level.CameraNewId });
            Static.Library.Dirty = true;
            UpdateCamerasList();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void propertyGrid4_Click(object sender, EventArgs e)
        {

        }

        private void properyListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count > 0)
            {
                var cam = listView5.SelectedItems[0].Tag as Camera;
                camera.CopyFrom(cam);
            }
        }
        bool oldStyleDraw = true;
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            oldStyleDraw = checkBox4.Checked;
        }
    }
}
