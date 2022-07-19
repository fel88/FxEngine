using FxEngine.Gui;
using System;
using System.Windows.Forms;

namespace FxEngineEditor
{
    public partial class GuiEditor : Form
    {
        public GuiEditor()
        {
            InitializeComponent();
            OpenTK.GLControl c = new OpenTK.GLControl();
            c.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(c, 0, 1);
            dc.GameWindow = c;
                        
            NativeGlGuiElement.Drawer = new NativeDrawProvider(dc);

            gs.GuiElements.Add(new NativeButtonCore()
            {
                Caption="exit",
                Rect=new GuiBounds(100,100,300,50),
            });

        }

        GuiTestScreen gs = new GuiTestScreen();

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        GlControlDrawingContext dc = new GlControlDrawingContext();
        private void timer1_Tick(object sender, EventArgs e)
        {            
            gs.Draw(dc);
        }
    }
}
