using System.Windows.Forms;

namespace FxEngineEditor
{
    public static class Extensions
    {
        public static ListViewItem FirstSelected(this ListView lv)
        {
            if (lv.SelectedItems.Count > 0)            
                return lv.SelectedItems[0];
            
            return null;
        }        
    }
}
