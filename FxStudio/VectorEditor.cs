using OpenTK.Mathematics;
using System;
using System.Windows.Forms;

namespace FxEngineEditor
{
    public partial class VectorEditor : Form,IValEnter
    {
        public Vector3 Vector { get; internal set; }

        public VectorEditor()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Vector3 vv = new Vector3();
            vv.X = float.Parse(textBox1.Text);
            vv.Y = float.Parse(textBox2.Text);
            vv.Z = float.Parse(textBox3.Text);
            Vector = vv;
            DialogResult = DialogResult.OK;
            Close();
        }

        public void SetVal(object o)
        {
            var v = (Vector3)o;
            textBox1.Text = v.X + "";
            textBox2.Text = v.Y + "";
            textBox3.Text = v.Z + "";
        }

        
    }

    public interface IValEnter
    {
        void SetVal(object ob);
    }
}
