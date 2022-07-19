using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using OpenTK;

namespace FxEngineEditor
{
    public class ProperyListView : ListView
    {
        public ProperyListView()
        {
            GridLines = true;
            FullRowSelect = true;
            HideSelection = false;
            View = View.Details;

            Columns.Add("prop", 150);
            Columns.Add("value", 150);

            SelectedIndexChanged += ProperyListView_SelectedIndexChanged;
            MouseDoubleClick += ProperyListView_MouseDoubleClick;
        }

        private void ProperyListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SelectedItems.Count > 0)
            {
                ListViewHitTestInfo hit = HitTest(e.Location);
                int columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
                var tag = SelectedItems[0].Tag as PropInfo;
                if (columnindex == 1)
                {
                    if (tag.GetValue().GetType() == typeof(Vector3))
                    {
                        VectorEditor ven = new FxEngineEditor.VectorEditor();

                        ven.SetVal((Vector3)tag.GetValue());
                        ven.StartPosition = FormStartPosition.Manual;
                        ven.Location = e.Location;
                        if (ven.ShowDialog() == DialogResult.OK)
                        {
                            tag.SetValue(ven.Vector);
                            SelectedItems[0].SubItems[1].Text = tag.GetValue().ToString();
                        }
                        return;
                    }
                    {
                        ValueEnter ven = new FxEngineEditor.ValueEnter();

                        ven.SetVal(tag.GetValue());
                        ven.StartPosition = FormStartPosition.Manual;
                        ven.Location = e.Location;
                        if (ven.ShowDialog() == DialogResult.OK)
                        {
                            tag.SetValue(ven.ValStr);
                            SelectedItems[0].SubItems[1].Text = tag.GetValue().ToString();
                        }

                        return;
                    }
                }

                if (SelectedItems[0].Text == "..")
                {
                    SetObject(SelectedItems[0].Tag, Stack.Last());
                    Stack.RemoveAt(Stack.Count - 1);
                }
                else
                {
                    Stack.Add(Tag);
                    SetObject(tag.Value, Tag);
                }
            }
        }

        private void ProperyListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public class PropInfo
        {
            public object Object;
            public MemberInfo Member;
            public object Value;

            internal void SetValue(string valStr)
            {
                if (Member is FieldInfo)
                {
                    var t = (Member as FieldInfo).FieldType;
                    if (t == typeof(int))
                    {
                        (Member as FieldInfo).SetValue(Object, int.Parse(valStr));
                    }
                    if (t == typeof(float))
                    {
                        (Member as FieldInfo).SetValue(Object, float.Parse(valStr));
                    }
                    if (t == typeof(bool))
                    {
                        (Member as FieldInfo).SetValue(Object, bool.Parse(valStr));
                    }
                }
                if (Member is PropertyInfo)
                {
                    var t = (Member as PropertyInfo).PropertyType;
                    if (t == typeof(int))
                    {
                        (Member as PropertyInfo).SetValue(Object, int.Parse(valStr));
                    }
                    if (t == typeof(float))
                    {
                        (Member as PropertyInfo).SetValue(Object, float.Parse(valStr));
                    }
                    if (t == typeof(bool))
                    {
                        (Member as PropertyInfo).SetValue(Object, bool.Parse(valStr));
                    }
                }
            }

            public object GetValue()
            {
                if (Member is FieldInfo)
                {
                    return (Member as FieldInfo).GetValue(Object);
                }
                if (Member is PropertyInfo)
                {
                    return (Member as PropertyInfo).GetValue(Object);
                }
                return null;

            }

            public void SetValue(object obj)
            {
                if (Member is FieldInfo)
                {
                    var t = (Member as FieldInfo).FieldType;
                    (Member as FieldInfo).SetValue(Object, obj);

                }
                if (Member is PropertyInfo)
                {
                    var t = (Member as PropertyInfo).PropertyType;
                    (Member as PropertyInfo).SetValue(Object, obj);
                }
            }
        }
        List<object> Stack = new List<object>();
        public void SetObject(object obj, object parent = null)
        {
            Tag = obj;
            var tp = obj.GetType();
            Items.Clear();
            Items.Add(new ListViewItem(new string[] { ".." }) { Tag = parent });

            foreach (var item in tp.GetProperties())
            {
                try
                {
                    var v = item.GetValue(obj);

                    Items.Add(new ListViewItem(new string[] { item.Name, v + "" })
                    {
                        BackColor = System.Drawing.Color.AliceBlue,
                        Tag = new PropInfo()
                        {
                            Object = obj,
                            Member = item,
                            Value = v
                        }
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
                    var v = item.GetValue(obj);
                    Items.Add(new ListViewItem(new string[] { item.Name, v + "" })
                    {
                        Tag = new PropInfo()
                        {
                            Object = obj,
                            Member = item,
                            Value = v
                        }
                    });
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
