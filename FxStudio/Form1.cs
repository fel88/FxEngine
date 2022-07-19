using FxEngine;
using System;
using System.Linq;
using System.Windows.Forms;

namespace FxEngineEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            config.Load();
            UpdateRecentsList();
        }

        public void UpdateRecentsList()
        {
            recentLibsToolStripMenuItem.DropDownItems.Clear();
            foreach (var item in config.RecentLibraries)
            {
                var tl = new ToolStripMenuItem(item) { Tag = item };
                recentLibsToolStripMenuItem.DropDownItems.Add(tl);
                tl.Click += Tl_Click;
            }
        }

        private void Tl_Click(object sender, EventArgs e)
        {
            var ss = (sender as ToolStripMenuItem);
            var str = ss.Tag as string;
            LoadLibrary(str);
        }

        Config config = new Config();

        

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenChild<PrefabEditor>();
        }

        public void OpenChild<T>() where T : Form, new()
        {           
            if (MdiChildren.Any(z => z is T))
            {
                var s = MdiChildren.First(z => z is T);
                s.Show();
                s.Activate();
                s.Focus();
                return;

            }
            T p = new T();
            p.MdiParent = this;
            p.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            OpenChild<TileEditor>();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            OpenChild<LevelEditor>();
        }

        private void dfsdfToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void sdfsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".xml";
            sfd.Filter = "FxEngine Library (*.xml)|*.xml";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Static.Library = new GameResourcesLibrary();
                Static.Library.Save(sfd.FileName);
                toolStripButton2.Enabled = true;
                toolStripButton1.Enabled = true;
                toolStripButton3.Enabled = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Static.Library.Save(sfd.FileName);
            }
        }


        public void LoadLibrary(string path)
        {
            Static.Library = GameResourcesLibrary.LoadFromXml(path);
            
            Text = "FxStudio Assets Editor: " + path + " ;  " + Static.Library.Name;
            toolStripButton2.Enabled = true;
            toolStripButton1.Enabled = true;
            toolStripButton3.Enabled = true;
            if (!config.RecentLibraries.Contains(path))
            {
                config.RecentLibraries.Add(path);
                config.IsDirty = true;
            }

            UpdateRecentsList();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "FxEngine Linrary (*.xml)|*.xml";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadLibrary(ofd.FileName);
            }
        }

        private void toolStripButton2_MouseHover(object sender, EventArgs e)
        {
        }

        private void toolStripButton2_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton2.Enabled = Static.Library != null;

        }

        private void toolStripButton3_MouseHover(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_MouseEnter(object sender, EventArgs e)
        {
            toolStripButton3.Enabled = Static.Library != null;

        }

        private void toolStripButton1_MouseEnter(object sender, EventArgs e)
        {
            //toolStripButton1.Enabled = Static.Library != null;

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            OpenChild<LibrarySettings>();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            OpenChild<GuiEditor>();

        }

        private void recentLibsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        public DialogResult ShowQuestion(string text)
        {
            return MessageBox.Show(text, Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (config.IsDirty)
            {
                config.Save();
            }
            
            if (Static.Library != null && Static.Library.Dirty)
            {
                switch (ShowQuestion($"Save library changes: {Static.Library.Name}?"))
                {
                    case DialogResult.Yes:
                        Static.Library.Save(Static.Library.LibraryPath);
                        break;
                }                
            }
        }

        private void Form1_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            TileSetEditor tse = new TileSetEditor();
            tse.MdiParent = this;
            tse.Show();

        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            OpenChild<AssetNavigator>();
        }
    }
}
