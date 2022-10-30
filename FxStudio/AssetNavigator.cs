using FxEngine.Assets;
using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FxEngineEditor
{
    public partial class AssetNavigator : Form
    {
        public AssetNavigator()
        {
            InitializeComponent();
        }
        AssetArchive Archive;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter="FxEngine library (*.)"
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Archive = new AssetArchive();
                Archive.LoadFromBinaryFile(ofd.FileName);
                UpdateFilesList();
            }
        }
        /*public void AppendListDir(TreeNode tn, AssetDirectory d)
        {
            foreach (var item in d.Directories)
            {
                tn.Nodes.Add(new TreeNode(item.Name) { Tag = item });
            }
            foreach (var item in Archive.Files)
            {
                treeView1.Nodes.Add(new TreeNode(item.Path) { Tag = item });
            }
        }*/

        public void UpdateFilesList()
        {
            listView1.Items.Clear();


            /*foreach (var item in Archive.Directories)
            {
                var tn1 = new TreeNode(item.Name) { Tag = item };
                treeView1.Nodes.Add(tn1);
                AppendListDir(tn1, Archive);
            }*/
            foreach (var item in Archive.Files)
            {
                if (!item.Path.ToLower().Contains(textBox1.Text.ToLower())) continue;
                listView1.Items.Add(new ListViewItem(new string[] { item.Path, item.Data.Length / 1024 + "Kb" }) { Tag = item });
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var fl1 = Archive.Files.First(z => z.Path.EndsWith("xml"));
            var enc = new UTF8Encoding();
            var str = enc.GetString(fl1.Data);
            var doc = XDocument.Parse(str);
            foreach (var item in doc.Descendants("model"))
            {
                if (item.Attribute("path") == null) continue;
                var p = item.Attribute("path").Value;
                var f = new FileInfo(p);
                item.Attribute("path").Value = f.Name;
                //item.SetAttributeValue(item.Attribute("path").Name, f.Name);                   

            }
            foreach (var item in doc.Descendants("tile"))
            {
                if (item.Attribute("path") == null) continue;
                var p = item.Attribute("path").Value;
                var f = new FileInfo(p);
                item.Attribute("path").Value = f.Name;
                //item.SetAttributeValue(item.Attribute("path").Name, f.Name);                   

            }
            var mems = new MemoryStream();
            var tx = new StreamWriter(mems, enc);
            doc.Save(tx);
            fl1.Data = mems.ToArray();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var file = listView1.SelectedItems[0].Tag as AssetFile;

                if (file.Path.EndsWith("xml") || file.Path.EndsWith("obj") || file.Path.EndsWith("txt") || file.Path.EndsWith("mtl"))
                {
                    var en = new UTF8Encoding();
                    Clipboard.SetText(en.GetString(file.Data));
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var file = listView1.SelectedItems[0].Tag as AssetFile;
                if (file.Path.EndsWith("xml") || file.Path.EndsWith("obj") || file.Path.EndsWith("txt") || file.Path.EndsWith("mtl") || file.Path.EndsWith("dae"))
                {
                    var en = new UTF8Encoding();
                    panel1.Controls.Clear();
                    var rtb = new RichTextBox() { Dock = DockStyle.Fill };

                    rtb.Text = en.GetString(file.Data);
                    panel1.Controls.Add(rtb);
                }
                if (file.Path.EndsWith("png") || file.Path.EndsWith("jpg") || file.Path.EndsWith("bmp"))
                {
                    var en = new UTF8Encoding();
                    panel1.Controls.Clear();
                    var rtb = new PictureBox() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom };
                    var ms = new MemoryStream(file.Data);
                    var img = Image.FromStream(ms);
                    rtb.Image = img;
                    panel1.Controls.Add(rtb);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateFilesList();
        }

        private void binaryAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Archive.SaveToFile(sfd.FileName);
            }
        }

        private void archivedAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "FxEngine library (*.fxl)|*.fxl";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //todo: modify lib.xml paths here!!
                using (var fileStream = new FileStream(sfd.FileName, FileMode.Create))
                {
                    using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in Archive.Files)
                        {
                            var fn = Path.GetFileName(item.Name);
                            var demoFile = archive.CreateEntry(fn);
                            using (MemoryStream mss = new MemoryStream(item.Data))
                            {
                                using (var entryStream = demoFile.Open())
                                {
                                    mss.CopyTo(entryStream);
                                }                                                    
                            }
                        }                       
                    }
                }                           
            }
        }
    }
}
