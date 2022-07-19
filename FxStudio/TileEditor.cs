using FxEngine.Tiles;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FxEngineEditor
{
    public partial class TileEditor : Form
    {
        GLControl gl;

        public TileEditor()
        {
            InitializeComponent();
            UpdateList();

            gl = new OpenTK.GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8));
            // 
            gl.Margin = new Padding(0);

            Controls.Add(gl);
            gl.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(gl, 0, 1);
            this.MouseWheel += Form1_MouseWheel;

            gl.Resize += gl_Resize;
            gl.Paint += gl_Paint;
            gl.Load += gl_Load;

            Application.Idle += Application_Idle;

        }
        void gl_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(Color.SkyBlue);
            SetupViewport();
        }

        void Render()
        {
            if (!Static.Library.Inited)
            {
                Static.Library.Init();
            }
            gl.MakeCurrent();

            if (!loaded)
                return;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);
            SetupViewport();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();


            lookat = Matrix4.LookAt(camFrom, camTo, camUp);
            GL.MultMatrix(ref lookat);

            if (gl.Focused)
                GL.Color3(Color.Yellow);
            else
            {

                GL.Color3(Color.Blue);
            }


            //foreach (var mi in Stuff.Models)
            {
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

                GL.LineWidth(1);
                GL.Color3(Color.White);
                GL.PushMatrix();
                GL.Rotate(rotation, Vector3.UnitZ);

                if (SelectedTile != null)
                {
                    if (checkBox1.Checked)
                    {
                        for (int i = -10; i < 10; i++)
                        {
                            for (int j = -10; j < 10; j++)
                            {
                                SelectedTile.Position = new PointF(i * SelectedTile.bmpt.Width, j * SelectedTile.bmpt.Height);
                                SelectedTile.Scale = 1;
                                SelectedTile.Draw();
                            }
                        }
                    }
                    else
                    {
                        SelectedTile.Position = new PointF();
                        SelectedTile.Scale = 1;
                        SelectedTile.Draw();
                    }
                }
                GL.PopMatrix();
            }

            gl.SwapBuffers();
        }

        public Tile SelectedTile = null;

        void gl_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void SetupViewport()
        {
            int w = gl.Width;
            int h = gl.Height;
            var aspect = (float)w / (float)h;
            bool orthoView = false;
            int r = 20;
            if (orthoView)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
            }

            else
            {
                Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), aspect, 1, 10000);

                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.MultMatrix(ref perspective);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                GL.MultMatrix(ref lookat);
            }

            GL.Viewport(0, 0, w, h);
        }

        bool loaded = false;
        void gl_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;
            SetupViewport();
        }
        private Matrix4 lookat;

        private Vector3 camFrom = new Vector3(170, 170, 170);
        private Vector3 camTo;
        private Vector3 camUp = Vector3.UnitZ;
        private float rotation = 0; void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            float zoomK = 10;
            if (
     gl.ClientRectangle.IntersectsWith(new Rectangle(gl.PointToClient(Cursor.Position),
                                                              new Size(1, 1))))
            {
                if (e.Delta > 0)
                {
                    var dir = camTo - camFrom;
                    dir.Normalize();
                    camFrom += dir * zoomK;

                }
                else
                {
                    var dir = camTo - camFrom;
                    dir.Normalize();
                    camFrom -= dir * zoomK;
                }

                gl.Invalidate();
            }

        }
        private void Application_Idle(object sender, EventArgs e)
        {
            gl.Invalidate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var tl = new FxEngine.Tiles.Tile();
                tl.Init(ofd.FileName);
                if (Static.Library.Tiles.Any())
                {
                    tl.Id = Static.Library.Tiles.Max(z => z.Id) + 1;
                }

                Static.Library.AddTile(tl);
                UpdateList();
            }
        }

        public void UpdateList()
        {
            listView1.Items.Clear();
            foreach (var item in Static.Library.Tiles)
            {
                listView1.Items.Add(new ListViewItem(new string[] { item.Id + "", item.Name, item.Path }) { Tag = item });
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var s = listView1.SelectedItems[0].Tag as Tile;

                SelectedTile = s;
                propertyGrid1.SelectedObject = s;


            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            camFrom = new Vector3(0, 0, 100);
            camUp = Vector3.UnitY;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            camFrom = new Vector3(100, 100, 100);
            camUp = Vector3.UnitZ;
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var t = listView1.SelectedItems[0].Tag as Tile;
                Static.Library.RemoveTile(t);
                UpdateList();
            }
        }
    }
}
