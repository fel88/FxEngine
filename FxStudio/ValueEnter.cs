using System;
using System.Windows.Forms;

namespace FxEngineEditor
{
    public partial class ValueEnter : Form,IValEnter
    {
        public ValueEnter()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public string ValStr;
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ValStr = textBox1.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        
        public void SetVal(object p)
        {
            textBox1.Text = p.ToString();            
        }
    }
}
