using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Gui
{
    public class GuiLoader
    {
        private static GlGuiElement[] ParseElement(XElement item, object handler, GlGuiElement parent = null)
        {
            List<GlGuiElement> ret = new List<GlGuiElement>();
            if (item.Name == "textBox")
            {
                var eid = item.Attribute("id").Value;
                var but = new NativeTextBox() { Parent = parent };
                but.Id = eid;
                but.Text = item.Attribute("text").Value;
                var aa = item.Attribute("rect").Value.Split(';').Select(int.Parse).ToArray();
                var bb = item.Attribute("anchors").Value.Split(';').ToArray();
                but.Rect = new GuiBounds(aa[0], aa[1], aa[2], aa[3]);
                foreach (var bitem in bb)
                {

                    switch (bitem)
                    {
                        case "bottom":
                            but.Rect.Anchor |= GuiAnchor.Bottom;
                            break;
                        case "right":
                            but.Rect.Anchor |= GuiAnchor.Right;
                            break;
                        case "left":
                            but.Rect.Anchor |= GuiAnchor.Left;
                            break;
                    }
                }
                var w1 = handler.GetType().GetMethods().Where(z =>
                {
                    var rr = z.GetCustomAttributes(typeof(ClickProcessorAttribute), true);
                    return rr.Any();
                    //z.GetCustomAttribute(typeof(ClickProcessorAttribute)) != null

                }).ToArray();
                foreach (var witem in w1)
                {
                    var rr = witem.GetCustomAttributes(typeof(ClickProcessorAttribute), true);

                    //var id = (witem.GetCustomAttribute(typeof(ClickProcessorAttribute)) as ClickProcessorAttribute).Id;
                    var id = (rr[0] as ClickProcessorAttribute).Id;
                    if (id.StartsWith("#"))
                    {
                        id = id.Substring(1);
                    }
                    if (id == eid)
                    {
                        but.TextChanged = (x) => { witem.Invoke(handler, new object[] { x }); };
                        break;
                    }
                }
                ret.Add(but);
            }
            if (item.Name == "button")
            {
                var eid = item.Attribute("id").Value;
                var but = new NativeButton() { Parent = parent };
                but.Id = eid;
                but.Caption = item.Attribute("caption").Value;
                var aa = item.Attribute("rect").Value.Split(';').Select(int.Parse).ToArray();
                var bb = item.Attribute("anchors").Value.Split(';').ToArray();
                but.Rect = new GuiBounds(aa[0], aa[1], aa[2], aa[3]);
                foreach (var bitem in bb)
                {

                    switch (bitem)
                    {
                        case "bottom":
                            but.Rect.Anchor |= GuiAnchor.Bottom;
                            break;
                        case "right":
                            but.Rect.Anchor |= GuiAnchor.Right;
                            break;
                        case "left":
                            but.Rect.Anchor |= GuiAnchor.Left;
                            break;
                    }
                }
                var w1 = handler.GetType().GetMethods().Where(z =>
                {
                    var rr = z.GetCustomAttributes(typeof(ClickProcessorAttribute), true);
                    return rr.Any();
                    //z.GetCustomAttribute(typeof(ClickProcessorAttribute)) != null

                }).ToArray();
                foreach (var witem in w1)
                {
                    var rr = witem.GetCustomAttributes(typeof(ClickProcessorAttribute), true);

                    //var id = (witem.GetCustomAttribute(typeof(ClickProcessorAttribute)) as ClickProcessorAttribute).Id;
                    var id = (rr[0] as ClickProcessorAttribute).Id;
                    if (id.StartsWith("#"))
                    {
                        id = id.Substring(1);
                    }
                    if (id == eid)
                    {
                        but.Click = () => { witem.Invoke(handler, null); };
                        break;
                    }
                }
                ret.Add(but);
            }
            if (item.Name == "stackPanel")
            {
                var eid = item.Attribute("id").Value;
                var but = new NativeStackPanel() { Parent = parent };
                but.Id = eid;
                but.Title = item.Attribute("title").Value;
                var aa = item.Attribute("rect").Value.Split(';').Select(int.Parse).ToArray();
                var bb = item.Attribute("anchors").Value.Split(';').ToArray();
                but.Rect = new GuiBounds(aa[0], aa[1], aa[2], aa[3]);
                foreach (var bitem in bb)
                {

                    switch (bitem)
                    {
                        case "left":
                            but.Rect.Anchor |= GuiAnchor.Left;
                            break;
                        case "bottom":
                            but.Rect.Anchor |= GuiAnchor.Bottom;
                            break;
                        case "right":
                            but.Rect.Anchor |= GuiAnchor.Right;
                            break;
                    }
                }

                ret.Add(but);

                foreach (var xitem in item.Elements())
                {
                    but.Childs.AddRange(ParseElement(xitem, handler, but));
                }
            }
            if (item.Name == "panel")
            {
                var eid = item.Attribute("id").Value;
                var but = new NativePanel() { Parent = parent };
                but.Id = eid;
                but.Title = item.Attribute("title").Value;
                var aa = item.Attribute("rect").Value.Split(';').Select(int.Parse).ToArray();
                var bb = item.Attribute("anchors").Value.Split(';').ToArray();
                but.Rect = new GuiBounds(aa[0], aa[1], aa[2], aa[3]);
                foreach (var bitem in bb)
                {

                    switch (bitem)
                    {
                        case "left":
                            but.Rect.Anchor |= GuiAnchor.Left;
                            break;
                        case "bottom":
                            but.Rect.Anchor |= GuiAnchor.Bottom;
                            break;
                    }
                }

                ret.Add(but);

                foreach (var xitem in item.Elements())
                {
                    but.Childs.AddRange(ParseElement(xitem, handler, but));
                }
            }
            return ret.ToArray();
        }
        public static GlGuiElement[] LoadGui(string path, object handler)
        {
            var doc = XDocument.Load(path);
            var gui = doc.Elements("gui").First();
            List<GlGuiElement> elems = new List<GlGuiElement>();
            foreach (var item in gui.Elements())
            {
                elems.AddRange(ParseElement(item, handler));
            }

            return elems.ToArray();
        }

    }
}
