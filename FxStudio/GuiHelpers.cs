using System.Windows.Forms;

namespace FxEngineEditor
{
    public static class GuiHelpers
    {
        public static DialogResult ShowQuestion(string text,string title)
        {
            return MessageBox.Show(text, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }
        public static DialogResult ShowInfo(string text, string title)
        {
            return MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
