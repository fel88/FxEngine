using System;
using System.Drawing;
using System.Windows.Forms;

namespace FxEngineEditor
{
    public partial class TileSetEditor : Form
    {
        public TileSetEditor()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Bmp = Bitmap.FromFile(ofd.FileName) as Bitmap;
                propertyGrid1.SelectedObject = Bmp;
                var v = Bmp.VerticalResolution;
                var h = Bmp.HorizontalResolution;
                using (var gr = CreateGraphics())
                {
                    //Bmp.SetResolution(gr.DpiX, gr.DpiY);
                }
            }
        }

        Bitmap Bmp;

        TileSet Current = null;
        public void UpdateItems()
        {
            if (Current == null) return;
            listView1.Items.Clear();
            foreach (var item in Current.Items)
            {
                listView1.Items.Add(new ListViewItem(new string[] { item.Coord + " " + item.IsVertical }) { Tag = item });
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (Current == null) return;
            Current.Items.Add(new FxEngineEditor.TileSetSeparator());
            UpdateItems();
        }

        float zoom = 1;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            var gr = Graphics.FromImage(bmp);
            if (Bmp != null)
            {
                gr.DrawImage(Bmp, 0, 0);
            }
            if(Current!=null)
            foreach (var item in Current.Items)
            {
                if (item.IsVertical)
                {
                    gr.DrawLine(Pens.Black, item.Coord, 0, item.Coord, bmp.Height);
                }else
                {
                    gr.DrawLine(Pens.Black, 0,item.Coord, bmp.Width, item.Coord);
                }
            }
            pictureBox1.Image = bmp;
            gr.Dispose();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var sep = listView1.SelectedItems[0].Tag as TileSetSeparator;
                propertyGrid1.SelectedObject = sep;
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Current = new FxEngineEditor.TileSet();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {                
                Bmp.SetResolution(96, 96);
                Bmp.Save(sfd.FileName);                
            }
        }
    }
}
