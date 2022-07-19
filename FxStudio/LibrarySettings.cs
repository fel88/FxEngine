using System;
using System.Windows.Forms;

namespace FxEngineEditor
{
    public partial class LibrarySettings : Form
    {
        public LibrarySettings()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = Static.Library;
        }

        private void LibrarySettings_Load(object sender, EventArgs e)
        {

        }
    }
}
