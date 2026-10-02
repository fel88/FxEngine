using FxEngine;
using FxEngine.Fonts;
using OpenTK.Graphics.OpenGL;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            glControl = new OpenTK.GLControl.GLControl(new OpenTK.GLControl.GLControlSettings()
            {
                NumberOfSamples = 32
            });
            glControl.Dock = DockStyle.Fill;
            glControl.Paint += GlControl_Paint;
            Controls.Add(glControl);

        }

        FreeTypeTextRenderer textRenderer = new FreeTypeTextRenderer();
        FreeTypeTextRenderer textRenderer2 = new FreeTypeTextRenderer();
        FreeTypeTextRenderer textRenderer3 = new FreeTypeTextRenderer();
        bool first = true;
        private void GlControl_Paint(object? sender, PaintEventArgs e)
        {
            if (first)
            {
                var fontBytes = ResourceHelper.ReadResourceRaw("Consolas-Regular.ttf");
                var fontBytes2 = ResourceHelper.ReadResourceRaw("Verdana.ttf");

                textRenderer3.Init(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, fontBytes2, 48);
                textRenderer.Init(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, fontBytes, 48);
                textRenderer2.Init(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

                first = false;
            }

            GL.ClearColor(Color.RebeccaPurple);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var greenColor = new OpenTK.Mathematics.Vector3(0.3f, 1, 0.3f);

            textRenderer.RenderText("Hello world! 123.458", 10, 10, greenColor);
            textRenderer2.RenderText("Hello world! 123.458", 10, 100, greenColor);
            textRenderer3.RenderText("Hello world! 123.458", 10, 200, greenColor);
            glControl.SwapBuffers();
        }

        OpenTK.GLControl.GLControl glControl;
    }
}
