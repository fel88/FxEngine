using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FxEngine.Gui
{
    public class NativePropertyDebugPanel : NativePanel
    {

        public object Object;

        public Action<NativePropertyDebugPanel> ObjectChanged;
        public List<PropertyLabelItem> Labels = new List<PropertyLabelItem>();
        public void SetObject(object obj)
        {
          
            Object = obj;
            if (ObjectChanged != null)
            {
                ObjectChanged(this);
            }
            if (!IsExpanded) return;
            var tp = obj.GetType();
            Labels.Clear();
            Childs.Clear();
            Labels.Add(new PropertyLabelItem()
            {
                Text = Object.GetType().Name + " :" + Object.ToString(),
                Object = Object,
                Updater = (x) => { x.Text = x.Object.GetType().Name + " :" + x.Object.ToString(); }

            });
            foreach (var item in tp.GetProperties())
            {
                try
                {
                    var val = item.GetValue(obj, new object[] { });
                    if (val == null)
                    {
                        val = "null";
                    }
                    Labels.Add(new PropertyLabelItem()
                    {
                        Text = item.Name + ": " + val.ToString()
                    ,
                        Prop = item
                    });

                }
                catch (Exception ex)
                {

                }
            }
            foreach (var item in tp.GetFields())
            {
                try
                {
                    var val = item.GetValue(obj);
                    if (val == null)
                    {
                        val = "null";
                    }
                    Labels.Add(new PropertyLabelItem()
                    {
                        Text = item.Name + ": " + val.ToString()
                    ,
                        Field = item
                    });

                }
                catch (Exception ex)
                {

                }
            }
            if (Object is IList)
            {
                var list = (Object as IList);
                foreach (var item in list)
                {
                    try
                    {
                        var val = item;
                        if (val == null)
                        {
                            val = "null";
                        }
                        Labels.Add(new PropertyLabelItem()
                        {
                            Text = val.ToString()

                        });

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            foreach (var item in Labels)
            {
                Childs.Add(item);
            }

            UpdateHeight();
        }

        public void UpdateHeight()
        {
            int yy = 20;
            for (int i = 0; i < Labels.Count; i++)
            {
                var rect = Labels[i].Rect;
                rect.Height = 20;
                rect.Width = 100;
                rect.YOffset = yy;
                yy += 20;
                rect.XOffset = 5;
            }
            if (IsExpanded)
            {
                Rect.Height = 20 + yy;
            }
        }

        public override void Update(BaseGlDrawingContext dc)
        {
            if (Object != null)
            {
                var tp = Object.GetType();
                int index = 0;
                foreach (var item in Labels)
                {
                    try
                    {
                        if (item.Prop != null)
                        {
                            var val = item.Prop.GetValue(Object, new object[] { });
                            if (val == null)
                            {
                                val = "null";
                            }
                            item.Text = item.Prop.Name + ": " + val.ToString();
                        }
                        if (item.Field != null)
                        {
                            var val = item.Field.GetValue(Object);
                            if (val == null)
                            {
                                val = "null";
                            }
                            item.Text = item.Field.Name + ": " + val.ToString();
                        }
                        if (item.Updater != null)
                        {
                            item.Updater(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (item.Prop != null)
                        {
                            item.Text = item.Prop.Name + ": exception";
                        }
                        if (item.Field != null)
                        {
                            item.Text = item.Field.Name + ": exception";
                        }
                    }
                }
                UpdateHeight();
            }

            base.Update(dc);
        }
        public NativePropertyDebugPanel()
        {
            Title = "Properties";
            Anchor = GuiAnchor.Right;
            Rect = new GuiBounds(5, 250, 260, 80, GuiAnchor.Left);
        }
    }

    public class PropertyLabelItem : NativeLabel
    {
        public object Object;
        public PropertyInfo Prop;
        public FieldInfo Field;
        public Action<PropertyLabelItem> Updater;
    }
}
