using System;
using System.Runtime.InteropServices;

namespace FxEngine.Fonts
{
    public class GDI
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);



        [DllImport("gdi32.dll")]
        public static extern uint GetKerningPairsW(IntPtr hdc, uint nNumPairs, [Out] KERNINGPAIR[] lpkrnpair);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);
    }


}
