using FxEngine;
using FxEngine.Assets;
using FxEngine.Loaders.Collada;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FxEngineEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            config.Load();
            UpdateRecentsList();

            //init lib
            Static.Library = new GameResourcesLibrary();
            toolStripButton2.Enabled = true;
            toolStripButton1.Enabled = true;
            toolStripButton3.Enabled = true;
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

        public T OpenChild<T>() where T : Form, new()
        {
            if (MdiChildren.Any(z => z is T))
            {
                var s = MdiChildren.OfType<T>().First();
                s.Show();
                s.Activate();
                s.Focus();
                return s;

            }
            T p = new T();
            p.MdiParent = this;
            p.Show();
            return p;
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
            Static.Library = new GameResourcesLibrary();
            toolStripButton2.Enabled = true;
            toolStripButton1.Enabled = true;
            toolStripButton3.Enabled = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "FxEngine Library (*.xml)|*.xml|Archived FxEngine Library (*.fxl)|*.fxl";
            if (sfd.ShowDialog() != DialogResult.OK) return;

            Static.Library.Save(sfd.FileName);
        }


        public async Task LoadLibraryAsync(string path)
        {
            await Task.Run(() => { LoadLibrary(path); });
        }
        public void LoadLibrary(string path)
        {
            if (path.EndsWith(".fxl"))
                Static.Library = GameResourcesLibrary.LoadFromZipAsset(path);
            else
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

        private async void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All FxEngine Libraries formats (*.xml, *.fxl)|*.xml;*.fxl|Compressed FxEngine Library (*.fxl)|*.fxl";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            var w = OpenChild<PrefabEditor>();
            await LoadLibraryAsync(ofd.FileName);
            w.UpdatePrefabsList();
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
        void AppendDae(AssetArchive arch, ColladaModelBlueprint cmb)
        {
            var path = cmb.FilePath;
            arch.AppendFile(path);

            var doc = XDocument.Load(path);
            var fr = doc.Descendants().First();
            var v = fr.Attribute("xmlns").Value;

            var xn = XName.Get("init_from", v);

            foreach (var xitem in doc.Descendants(xn))
            {
                var a = xitem.Value;
                var fin = new FileInfo(path);
                var comb = Path.Combine(fin.DirectoryName, a);
                if (File.Exists(comb))
                {
                    arch.AppendFile(comb);
                }
            }
        }
        void AppendObj(AssetArchive arch, ObjModelBlueprint model)
        {
            var path = model.FilePath;
            foreach (var item in model.Objs)
            {
                foreach (var titem in item.mat.textures2)
                {
                    arch.AppendFile(titem.Value);
                }
            }
            arch.AppendFile(path);
            var nm1 = Path.GetFileNameWithoutExtension(path);
            var d = Path.GetDirectoryName(path);
            var mtlp = Path.Combine(d, nm1 + ".mtl");
            if (File.Exists(mtlp))
            {
                arch.AppendFile(mtlp);
            }
        }
        private void binaryAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "FxEngine asset (*.asset)|*.asset";
            if (sfd.ShowDialog() != DialogResult.OK) return;
            AssetArchive asset = new AssetArchive();
            asset.AppendFile(Static.Library.LibraryPath);
            var lib = Static.Library;

            //load dae/mtl and so on.
            foreach (var item in lib.Fonts)
            {
                asset.AppendFile(item.Path);
                var doc = XDocument.Load(item.Path);
                var f = doc.Descendants("root").First();
                var path1 = f.Attribute("image").Value;
                asset.AppendFile(Path.Combine(new FileInfo(item.Path).DirectoryName, path1));
            }
            foreach (var item in lib.Models)
            {
                //if (item.FilePath.EndsWith("obj"))
                if(item is ObjModelBlueprint omb)
                {
                    AppendObj(asset, omb);
                    continue;
                }
                //if (item.FilePath.EndsWith("dae"))
                if (item is ColladaModelBlueprint cmb)
                {
                    AppendDae(asset, cmb);
                    continue;
                }

                asset.AppendFile(item.FilePath);
            }
            foreach (var item in lib.Tiles)
            {
                asset.AppendFile(item.Path);
            }
            foreach (var item in lib.Sounds)
            {
                asset.AppendFile(item.Path);
            }
            asset.SaveToFile(sfd.FileName);
        }

        private void archivedAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "FxEngine zip-asset (*.fxl)|*.fxl";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            var lib = Static.Library;

            if (File.Exists(sfd.FileName))
            {
                //ask for replace. use autoDialog?
                File.Copy(sfd.FileName, sfd.FileName + ".backup", true);
                File.Delete(sfd.FileName);
            }

            using (var archive = ZipFile.Open(sfd.FileName, ZipArchiveMode.Create))
            {
                var ff = new FileInfo(Static.Library.LibraryPath);
                var txt = File.ReadAllText(ff.FullName);


                foreach (var item in lib.Fonts)
                {
                    var fff = new FileInfo(item.Path);
                    archive.CreateEntryFromFile(fff.FullName, fff.Name);
                    txt = txt.Replace(item.Path, fff.Name);


                    var doc = XDocument.Load(item.Path);
                    var f = doc.Descendants("root").First();
                    var path1 = f.Attribute("image").Value;

                    var imgPath = Path.Combine(new FileInfo(item.Path).DirectoryName, path1);
                    fff = new FileInfo(imgPath);
                    archive.CreateEntryFromFile(fff.FullName, fff.Name);
                    txt = txt.Replace(imgPath, fff.Name);
                }

                foreach (var item in lib.Models)
                {
                    var fff = new FileInfo(item.FilePath);
                    archive.CreateEntryFromFile(fff.FullName, fff.Name);
                    txt = txt.Replace(item.FilePath, fff.Name);
                }
                foreach (var item in lib.Tiles)
                {
                    var fff = new FileInfo(item.Path);
                    archive.CreateEntryFromFile(fff.FullName, fff.Name);
                    txt = txt.Replace(item.Path, fff.Name);

                }
                foreach (var item in lib.Tiles)
                {
                    var fff = new FileInfo(item.Path);
                    archive.CreateEntryFromFile(fff.FullName, fff.Name);
                    txt = txt.Replace(item.Path, fff.Name);

                }

                foreach (var item in lib.Sounds)
                {
                    var fff = new FileInfo(item.Path);
                    archive.CreateEntryFromFile(fff.FullName, fff.Name);
                    txt = txt.Replace(item.Path, fff.Name);
                }

                var xmlEntry = archive.CreateEntry(ff.Name);

                //repalce all pathes in xml here!!!

                using (var stream = xmlEntry.Open())
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(txt);
                    }
                    //    using (var stream2 = ff.OpenRead())
                    //      stream2.CopyTo(stream);
                }



            }


        }

        private void zipAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "FxEngine zip-asset (*.fxl)|*.fxl";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            var lib = Static.Library;

            if (File.Exists(sfd.FileName))
            {
                //ask for replace. use autoDialog?
                File.Copy(sfd.FileName, sfd.FileName + ".backup", true);
                File.Delete(sfd.FileName);
            }

            using (var archive = ZipFile.Open(sfd.FileName, ZipArchiveMode.Create))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\"?>");
                sb.AppendLine($"<library name=\"{Name}\">");
                sb.AppendLine($"<models>");
                foreach (var item in Static.Library.Models)
                {
                    var dir = Path.GetFileNameWithoutExtension(item.Name);
                    if (item is ObjModelBlueprint omb)
                    {
                        var mats = omb.Objs.Select(z => z.mat).Distinct().ToArray();
                        foreach (var mat in mats)
                        {
                            var mpath2 = Path.Combine(dir, Path.GetFileName(mat.FilePath));
                            var bts2 = File.ReadAllBytes(mat.FilePath);
                            using (var ms = new MemoryStream(bts2))
                            {
                                var ent1 = archive.CreateEntry(mpath2);
                                using (var stream = ent1.Open())
                                {
                                    ms.CopyTo(stream);
                                }
                            }

                            foreach (var titem in mat.TDesc)
                            {
                                var mpath3 = Path.Combine(dir, Path.GetFileName(titem.FilePath));
                                var bts3 = File.ReadAllBytes(titem.FilePath);
                                using (var ms = new MemoryStream(bts3))
                                {
                                    var ent1 = archive.CreateEntry(mpath3);
                                    using (var stream = ent1.Open())
                                    {
                                        ms.CopyTo(stream);
                                    }
                                }
                            }
                        }
                    }
                    if (item is ColladaModelBlueprint cmb)
                    {
                        foreach (var library in cmb.Model.Libraries)
                        {
                            if (library is ColladaImageLibrary cil)
                            {
                                foreach (var img in cil.Images)
                                {
                                    var mpath2 = Path.Combine(dir, Path.GetFileName(img.Source));

                                    var bts2 = File.ReadAllBytes(img.Source);
                                    using (var ms = new MemoryStream(bts2))
                                    {
                                        var ent1 = archive.CreateEntry(mpath2);
                                        using (var stream = ent1.Open())
                                        {
                                            ms.CopyTo(stream);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var mpath = Path.Combine(dir, Path.GetFileName(item.FilePath));
                    sb.AppendLine($"<model id=\"{item.Id}\" name=\"{item.Name}\" path=\"{mpath}\"/>");
                    var bts = File.ReadAllBytes(item.FilePath);
                    using (var ms = new MemoryStream(bts))
                    {
                        var ent1 = archive.CreateEntry(mpath);
                        using (var stream = ent1.Open())
                        {
                            ms.CopyTo(stream);
                        }
                    }
                }
                sb.AppendLine($"</models>");
                sb.AppendLine($"<tiles>");
                foreach (var item in Static.Library.Tiles)
                {
                    var mpath = Path.Combine("Tiles", Path.GetFileName(item.Path));
                    sb.AppendLine($"<tile id=\"{item.Id}\" name=\"{item.Name}\" path=\"{mpath}\"/>");
                    var readPath = Path.Combine(Path.GetDirectoryName(lib.LibraryPath), item.Path);
                    var bts = File.ReadAllBytes(readPath);
                    using (var ms = new MemoryStream(bts))
                    {
                        var ent1 = archive.CreateEntry(mpath);
                        using (var stream = ent1.Open())
                        {
                            ms.CopyTo(stream);
                        }
                    }
                }
                sb.AppendLine($"</tiles>");
                sb.AppendLine($"<sounds>");

                foreach (var item in Static.Library.Sounds)
                {
                    var mpath = Path.Combine("Sounds", Path.GetFileName(item.Path));
                    sb.AppendLine($"<sound id=\"{item.Id}\" name=\"{item.Name}\" path=\"{mpath}\"/>");
                    var readPath = Path.Combine(Path.GetDirectoryName(lib.LibraryPath), item.Path);
                    var bts = File.ReadAllBytes(readPath);
                    using (var ms = new MemoryStream(bts))
                    {
                        var ent1 = archive.CreateEntry(mpath);
                        using (var stream = ent1.Open())
                        {
                            ms.CopyTo(stream);
                        }
                    }
                }

                sb.AppendLine($"</sounds>");
                sb.AppendLine($"<levels>");
                foreach (var level in Static.Library.Levels)
                {
                    level.StoreXml(sb);
                }
                sb.AppendLine($"</levels>");
                sb.AppendLine("</library>");

                var ent = archive.CreateEntry("lib.xml");
                using (var stream = ent.Open())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(sb.ToString());
                    }
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
