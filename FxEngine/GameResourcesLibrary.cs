using FxEngine.Assets;
using FxEngine.Gui;
using FxEngine.Loaders.Collada;
using FxEngine.Tiles;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FxEngine.Cameras;
using FxEngine.Loaders.OBJ;
using System.IO.Compression;

namespace FxEngine
{
    public class GameResourcesLibrary
    {
        public bool Inited = false;
        public string Name { get; set; }
        public bool Dirty = false;
        public List<string> LoadedObjs = new List<string>();
        public List<GlGuiElement> GuiElements = new List<GlGuiElement>();

        public string LibraryPath { get; set; }

        public void LoadModels()
        {
            var f = new FileInfo(Path.Combine(LibraryPath, "Library.xml"));
            var doc = XDocument.Load(f.FullName);
            foreach (var item in doc.Descendants("model"))
            {
                var path = item.Attribute("path").Value;
                var nm = item.Attribute("name").Value;
                models.Add(new ModelBlueprint(nm, path));
            }
        }

        public List<GameFont> Fonts = new List<GameFont>();
        private List<GameObject> GameObjects = new List<GameObject>();
        private List<ModelBlueprint> models = new List<ModelBlueprint>();
        public List<ModelPathItem> ModelsPathes = new List<ModelPathItem>();

        private List<GameLevel> levels = new List<GameLevel>();
        public List<TextureDescriptor> Textures { get; set; } = new List<TextureDescriptor>();
        private List<Tile> tiles = new List<Tile>();
        private List<GameSound> sounds = new List<GameSound>();

        public IEnumerable<Tile> Tiles
        {
            get
            {
                return tiles.ToArray();
            }
        }
        public IEnumerable<GameSound> Sounds
        {
            get
            {
                return sounds.ToArray();
            }
        }
        public GameLevel[] Levels
        {
            get
            {
                return levels.ToArray();
            }
        }
        public IEnumerable<ModelBlueprint> Models
        {
            get
            {
                return models.ToArray();
            }
        }

        public int ModelNewId
        {
            get
            {
                if (models.Any())
                {
                    return models.Max(z => z.Id) + 1;
                }
                return 0;
            }
        }

        public IDataProvider DataProvider { get; private set; }

        public void AddTile(Tile tl)
        {
            Dirty = true;
            tiles.Add(tl);
        }
        public void AddModel(ModelBlueprint tl)
        {
            Dirty = true;
            models.Add(tl);
        }
        public void AddLevel(GameLevel tl)
        {
            Dirty = true;
            levels.Add(tl);
        }
        public void RemoveTile(Tile tl)
        {
            Dirty = true;
            tiles.Remove(tl);
        }

        public static Matrix4 LoadTransforms(XElement elem)
        {
            Matrix4 mtr4 = Matrix4.Identity;
            foreach (var titem in elem.Descendants())
            {
                switch (titem.Name.LocalName)
                {
                    case "scale":
                        {
                            mtr4 = mtr4 * Matrix4.CreateScale(float.Parse(titem.Attribute("ratio").Value));
                        }
                        break;
                    case "translation":
                        {
                            var ar1 = titem.Attribute("v").Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            var ar2 = ar1.Select(z => float.Parse(z)).ToArray();
                            mtr4 = mtr4 * Matrix4.CreateTranslation(ar2[0], ar2[1], ar2[2]);
                        }
                        break;
                    case "rotation":
                        {
                            //var ar1 = titem.Attribute("v").Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            //var ar2 = ar1.Select(z => float.Parse(z)).ToArray();
                            var ang = float.Parse(titem.Attribute("v").Value);
                            var ar3 = titem.Attribute("axis").Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            var ar4 = ar3.Select(z => float.Parse(z)).ToArray();
                            mtr4 = mtr4 * Matrix4.CreateFromAxisAngle(new Vector3(ar4[0], ar4[1], ar4[2]), (float)(ang * Math.PI / 180.0f));
                        }
                        break;
                }
            }
            return mtr4;
        }

        public static float[] ParseFloats(string input)
        {
            return input.Split(new char[] { ' ', '\n' }, System.StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x.Replace(",", "."), CultureInfo.InvariantCulture)).ToArray();
        }

        public static int[] ParseInts(string input)
        {
            return input.Split(new char[] { ' ', '\n' }, System.StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x, CultureInfo.InvariantCulture)).ToArray();
        }

        public static GameResourcesLibrary LoadFromAsset(string fileName)
        {
            AssetArchive arc = new Assets.AssetArchive();
            arc.LoadFromBinaryFile(fileName);
            var fr = arc.Files.First(z => z.Path.EndsWith(".xml"));
            var ret = LoadFromXml(fr.Path, arc);
            return ret;
        }

        public static GameResourcesLibrary LoadFromZipAsset(string fileName, IDataProvider datap = null)
        {
            if (datap == null)
            {
                datap = new ZipAssetFilesystemDataProvider(fileName);
            }
            string libName = string.Empty;
            using (var zip = ZipFile.Open(fileName, ZipArchiveMode.Read))
            {
                var xmls = zip.Entries.First(z => z.Name.EndsWith(".xml") && z.Name.Contains("lib"));
                using (var stream = xmls.Open())
                {
                    libName = xmls.Name;
                }
            }
            var ret = LoadFromXml(libName, datap);
            return ret;
        }

        public static GameResourcesLibrary LoadFromXml(string fileName, IDataProvider datap = null)
        {
            if (datap == null)
            {
                datap = new PhysicalFilesystemDataProvider();
            }
            GameResourcesLibrary ret = new GameResourcesLibrary();
            ret.DataProvider = datap;
            ret.LibraryPath = fileName;
            //Directory.SetCurrentDirectory(new FileInfo(ret.LibraryPath).Directory.FullName);
            var doc = datap.LoadXml(fileName);
            var root = doc.Element("library");
            ret.Name = root.Attribute("name").Value;
            var models = root.Element("models");
            var fonts = root.Element("fonts");
            var gobjs = root.Element("gameObjects");
            if (fonts != null)
            {
                foreach (var item in fonts.Descendants("font"))
                {
                    var id = int.Parse((item.Attribute("id").Value));
                    var nm = (item.Attribute("name").Value);
                    var path = (item.Attribute("path").Value);
                    var path2 = Path.Combine(datap.GetDirectoryName(fileName), path);


                    var font = new FxEngine.GameFont() { Id = id, Name = nm, Path = path2 };
                    font.PreLoad(datap);

                    ret.Fonts.Add(font);
                }
            }

            foreach (var item in models.Descendants("model"))
            {
                var nm = (item.Attribute("name").Value);
                var filePath = (item.Attribute("path").Value);

                if (filePath.EndsWith("obj"))
                {
                    Matrix4 mtr4 = Matrix4.Identity;
                    var fr = item.Descendants().FirstOrDefault(z => z.Name == "transforms");
                    if (fr != null)
                    {
                        mtr4 = LoadTransforms(fr);
                    }
                    var path2 = Path.Combine(datap.GetDirectoryName(fileName), filePath);

                    var obj = ObjVolume.LoadFromFile(path2, mtr4, datap);
                    var t = new ModelBlueprint(nm, obj);
                    t.Id = int.Parse(item.Attribute("id").Value);
                    //t.FilePath = (item.Attribute("path").Value);
                    t.FilePath = path2;
                    ret.AddModel(t);
                }
                if (filePath.EndsWith("dae"))
                {
                    ModelBlueprint t = new ModelBlueprint(nm);
                    ret.AddModel(t);
                    t.Id = int.Parse(item.Attribute("id").Value);
                    t.FilePath = (item.Attribute("path").Value);
                    var path2 = Path.Combine(datap.GetDirectoryName(fileName), t.FilePath);
                    t.FilePath = path2;
                    t.Model = ColladaImporter.Load(path2, datap);
                }
            }

            #region tiles load
            var tiles = root.Element("tiles");
            if (tiles != null)
                foreach (var item in tiles.Elements("tile"))
                {
                    Tile t = new Tile();
                    ret.AddTile(t);
                    t.Id = int.Parse(item.Attribute("id").Value);

                    t.Path = Path.Combine(datap.GetDirectoryName(fileName), (item.Attribute("path").Value));
                    
                    t.Name = (item.Attribute("name").Value);
                    if (item.Attribute("w") != null)
                    {
                        var ww = int.Parse(item.Attribute("w").Value);
                        var hh = int.Parse(item.Attribute("h").Value);
                        t.Width = ww;
                        t.Height = hh;
                        t.ForceSizePreLoad = true;
                    }
                }

            #endregion
            #region sounds load

            var sounds = root.Element("sounds");
            if (sounds != null)
            {
                foreach (var item in sounds.Elements("sound"))
                {
                    GameSound t = new GameSound();
                    ret.sounds.Add(t);
                    t.Id = int.Parse(item.Attribute("id").Value);
                    t.Path = (item.Attribute("path").Value);
                    t.Name = (item.Attribute("name").Value);
                }
            }

            #endregion
            if (gobjs != null)
            {
                foreach (var item in gobjs.Descendants("gameObject"))
                {
                    var gid = int.Parse(item.Attribute("id").Value);
                    var nm = item.Attribute("name").Value;
                    var mdl = int.Parse(item.Attribute("model").Value);
                    ret.GameObjects.Add(new FxEngine.GameObject() { Id = gid, Name = nm, Model = ret.models.First(z => z.Id == mdl) });
                }
            }
            foreach (var item in root.Descendants("level"))
            {
                GameLevel t = new GameLevel();
                ret.AddLevel(t);
                t.Id = int.Parse(item.Attribute("id").Value);
                t.Name = (item.Attribute("name").Value);
                var modelTiles = item.Element("tiles");
                var levelGameObjects = item.Element("gameObjects");
                var modelModels = item.Element("models");
                var camerasModels = item.Element("cameras");
                if (levelGameObjects != null)
                {
                    foreach (var titem in levelGameObjects.Elements("gameObject"))
                    {
                        var id = int.Parse(titem.Attribute("id").Value);
                        var gid = int.Parse(titem.Attribute("gid").Value);
                        string nm = "";
                        if (titem.Attribute("name") != null)
                        {
                            nm = (titem.Attribute("name").Value);
                        }
                        t.GameObjectInstances.Add(new GameObjectInstance() { Id = id, GameObject = ret.GameObjects.First(z => z.Id == gid), Name = nm });
                        if (titem.Element("position") != null)
                        {
                            if (titem.Element("position").Elements().First().Name == "vector2d")
                            {
                                var fr1 = titem.Element("position").Elements().First();
                                var pos1 = ParseFloats(fr1.Attribute("pos").Value);
                                var v1 = new Vector2(pos1[0], pos1[1]);
                                t.GameObjectInstances.Last().Position = new Vector3(v1.X, v1.Y, 0);
                            }
                        }
                        if (titem.Element("transforms") != null)
                        {
                            var trans = LoadTransforms(titem);
                            t.GameObjectInstances.Last().Matrix = trans;
                        }
                    }
                }
                if (modelTiles != null)
                {
                    foreach (var titem in modelTiles.Elements("tileGrid"))
                    {
                        var tid = float.Parse(titem.Attribute("tileId").Value);
                        var w = int.Parse(titem.Attribute("w").Value);
                        var h = int.Parse(titem.Attribute("h").Value);
                        var cw = float.Parse(titem.Attribute("cellw").Value);
                        var ch = float.Parse(titem.Attribute("cellh").Value);
                        var shx = int.Parse(titem.Attribute("shx").Value);
                        var shy = int.Parse(titem.Attribute("shy").Value);
                        float zoffset = 0;
                        if (titem.Attribute("zoffset") != null)
                        {
                            zoffset = float.Parse(titem.Attribute("zoffset").Value);
                        }
                        var gap = float.Parse(titem.Attribute("gap").Value);
                        var scale = float.Parse(titem.Attribute("scale").Value);
                        var tile = ret.tiles.First(z => z.Id == tid);

                        for (int i = 0; i < w; i++)
                        {
                            for (int j = 0; j < h; j++)
                            {
                                t.Tiles.Add(new TileDrawItem()
                                {
                                    Tile = tile,
                                    Position3 = new Vector3((i + shx) * cw - gap, (j + shy) * ch - gap, zoffset),
                                    Scale = scale
                                });
                            }
                        }
                    }

                    foreach (var titem in modelTiles.Elements("tile"))
                    {
                        var tid = float.Parse(titem.Attribute("tileId").Value);
                        var xx = float.Parse(titem.Attribute("x").Value);
                        var yy = float.Parse(titem.Attribute("y").Value);
                        t.Tiles.Add(new FxEngine.Tiles.TileDrawItem() { Tile = ret.tiles.First(z => z.Id == tid), Position = new PointF(xx, yy) });
                    }
                }
                if (modelModels != null)
                {
                    foreach (var titem in modelModels.Elements("model"))
                    {
                        var mid = float.Parse(titem.Attribute("modelId").Value);
                        var id = int.Parse(titem.Attribute("id").Value);
                        var nm = (titem.Attribute("name").Value);

                        Matrix4 mtr = Matrix4.Identity;
                        if (titem.Attribute("matrix") != null)
                        {
                            mtr = ColladaStuff.MatrixFromArray((titem.Attribute("matrix").Value).Split(new char[] { ' ', '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x.Replace(",", "."), CultureInfo.InvariantCulture)).ToArray(), true);
                        }

                        var fr = titem.Descendants().FirstOrDefault(z => z.Name == "transforms");
                        if (fr != null)
                        {
                            mtr = Matrix4.Identity;//should it accum?
                            mtr = mtr * LoadTransforms(fr);
                        }
                        t.Models.Add(new ModelInstance()
                        {
                            Id = id,
                            Name = nm,
                            Blueprint = ret.Models.First(z => z.Id == mid),
                            Matrix = mtr
                        });
                    }

                    foreach (var titem in modelModels.Elements("modelGrid"))
                    {
                        var mid = float.Parse(titem.Attribute("modelId").Value);
                        var id = int.Parse(titem.Attribute("id").Value);
                        var nm = (titem.Attribute("name").Value);
                        var inc = (titem.Attribute("increment").Value).Split(new char[] { ' ' }).Select(z => float.Parse(z, CultureInfo.InvariantCulture)).ToArray();
                        var cnt = int.Parse(titem.Attribute("count").Value);


                        Matrix4 mtr = Matrix4.Identity;
                        if (tiles.Attribute("matrix") != null)
                        {
                            mtr = ColladaStuff.MatrixFromArray((titem.Attribute("matrix").Value).Split(new char[] { ' ', '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x.Replace(",", "."), CultureInfo.InvariantCulture)).ToArray(), true);
                        }

                        var fr = titem.Descendants().FirstOrDefault(z => z.Name == "transforms");
                        if (fr != null)
                        {
                            mtr = mtr * LoadTransforms(fr);
                        }

                        var incv3 = Matrix4.CreateTranslation(inc[0], inc[1], inc[2]);
                        for (int i = 0; i < cnt; i++)
                        {
                            mtr *= incv3;

                            t.Models.Add(new ModelInstance()
                            {
                                Id = id,
                                Name = nm,
                                Blueprint = ret.Models.First(z => z.Id == mid),
                                Matrix = mtr
                            });
                        }

                    }
                }
                if (camerasModels != null)
                {
                    foreach (var titem in camerasModels.Elements("camera"))
                    {

                        var id = int.Parse(titem.Attribute("id").Value);
                        var nm = (titem.Attribute("name").Value);
                        var from = (titem.Attribute("camFrom").Value).Split(new char[] { ' ', '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x.Replace(",", "."), CultureInfo.InvariantCulture)).ToArray();
                        var to = (titem.Attribute("camTo").Value).Split(new char[] { ' ', '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x.Replace(",", "."), CultureInfo.InvariantCulture)).ToArray();
                        var up = (titem.Attribute("camUp").Value).Split(new char[] { ' ', '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x.Replace(",", "."), CultureInfo.InvariantCulture)).ToArray();
                        var fovy = float.Parse(titem.Attribute("fovy").Value, CultureInfo.InvariantCulture);

                        t.Cameras.Add(new Camera()
                        {
                            Id = id,
                            Name = nm,
                            CamFrom = new Vector3(from[0], from[1], from[2]),
                            CamTo = new Vector3(to[0], to[1], to[2]),
                            CamUp = new Vector3(up[0], up[1], up[2]),
                            Fovy = fovy
                        });
                    }
                }
            }
            ret.Dirty = false;
            return ret;
        }

        public void Save(string path)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine($"<library name=\"{Name}\">");
            sb.AppendLine($"<models>");
            foreach (var item in models)
            {
                sb.AppendLine($"<model id=\"{item.Id}\" name=\"{item.Name}\" path=\"{item.FilePath}\"/>");
            }
            sb.AppendLine($"</models>");
            sb.AppendLine($"<tiles>");
            foreach (var item in Tiles)
            {
                sb.AppendLine($"<tile id=\"{item.Id}\" name=\"{item.Name}\" path=\"{item.Path}\"/>");
            }
            sb.AppendLine($"</tiles>");
            sb.AppendLine($"<levels>");
            foreach (var item in Levels)
            {
                sb.AppendLine($"<level id=\"{item.Id}\" name=\"{item.Name}\" >");
                sb.AppendLine("<tiles>");
                foreach (var fitem in item.Tiles)
                {
                    sb.AppendLine($"<tile tileId=\"{fitem.Tile.Id}\" x=\"{fitem.Position.X}\" y=\"{fitem.Position.Y}\" />");
                }
                sb.AppendLine("</tiles>");
                sb.AppendLine("<models>");
                foreach (var fitem in item.Models)
                {
                    sb.AppendLine($"<model id=\"{fitem.Id}\" name=\"{fitem.Name}\" modelId=\"{fitem.Blueprint.Id}\" matrix=\"{fitem.Matrix.ToXml()}\"  />");
                }
                sb.AppendLine("</models>");
                sb.AppendLine("<cameras>");
                foreach (var fitem in item.Cameras)
                {
                    var mode = fitem.IsOrtho ? "ortho" : "perspective";
                    sb.AppendLine($"<camera id=\"{fitem.Id}\" name=\"{fitem.Name}\" camFrom=\"{fitem.CamFrom.X};{fitem.CamFrom.Y};{fitem.CamFrom.Z}\"  camTo=\"{fitem.CamTo.X};{fitem.CamTo.Y};{fitem.CamTo.Z}\" camUp=\"{fitem.CamUp.X};{fitem.CamUp.Y};{fitem.CamUp.Z}\"  fovy=\"{fitem.Fovy}\" mode=\"{mode}\" />");
                }
                sb.AppendLine("</cameras>");
                sb.AppendLine("</level>");
            }
            sb.AppendLine($"</levels>");
            sb.AppendLine("</library>");

            File.WriteAllText(path, sb.ToString());
        }

        public void Init()
        {
            Inited = true;
            foreach (var item in tiles)
            {
                item.Init(item.Path);
            }
            foreach (var m in models)
            {
                if (m.Model != null)
                {
                    m.Model.InitLibraries();
                }
            }
        }

        public void LoadTiles(string dir)
        {

            var d = new DirectoryInfo(dir);
            foreach (var item in d.GetFiles())
            {
                if (item.Extension.Contains("png"))
                {
                    AddTile(new Tile());
                    Tiles.Last().Init(item.FullName);
                    var er = GL.GetError();
                }
            }
        }
    }
}

