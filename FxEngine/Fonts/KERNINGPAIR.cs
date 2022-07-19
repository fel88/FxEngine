using System;
using System.Runtime.InteropServices;

namespace FxEngine.Fonts
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KERNINGPAIR
    {
        public ushort wFirst; // might be better off defined as char
        public ushort wSecond; // might be better off defined as char
        public int iKernAmount;

        public KERNINGPAIR(ushort wFirst, ushort wSecond, int iKernAmount)
        {
            this.wFirst = wFirst;
            this.wSecond = wSecond;
            this.iKernAmount = iKernAmount;
        }

        public override string ToString()
        {
            return (String.Format("{{First={0}, Second={1}, Amount={2}}}", wFirst, wSecond, iKernAmount));
        }
    }


}
