using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FxEngine.Fonts
{
    public class KerningRoutine
    {
        public KerningRoutine(Graphics gr, Font font)
        {
            Init(gr, font);
        }
        ushort[] table;
        Int16[,] kernings;
        public RectangleF GetBound(Bitmap bmp)
        {

            int minx = bmp.Width;
            int miny = bmp.Height;
            int maxx = 0;
            int maxy = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    var p = bmp.GetPixel(i, j);
                    if (p.A > 128)
                    {
                        minx = Math.Min(minx, i);
                        miny = Math.Min(miny, j);
                        maxx = Math.Max(maxx, i);
                        maxy = Math.Max(maxy, j);
                    }
                }
            }
            return new RectangleF(minx, miny, maxx - minx + 1, maxy - miny + 1);
        }
        public RectangleF?[] offsets = new RectangleF?[65535];

        public RectangleF GetCharBitmapBound(char c)
        {
            if (offsets != null && offsets[c] != null)
            {
                return offsets[c].Value;
            }

            Bitmap bmp = new Bitmap(100, 100);
            GetCharBitmap(c, bmp);

            offsets[c] = GetBound(bmp);
            return offsets[c].Value;
        }
        public string Charset = charsetRussian + charsetNumber + charsetLatin;
        public void Init(Graphics _gr, Font _font)
        {
            gr = _gr;
            font = _font;
            table = BuildTable(Charset);
            int charCount = 256;
            kernings = new short[charCount, charCount];
            for (int x = 0; x < charCount; x++) for (int y = 0; y < charCount; y++) kernings[x, y] = 0;


            IntPtr gdiFont = font.ToHfont();
            IntPtr hdc = gr.GetHdc();
            IntPtr gdiPrevFont = GDI.SelectObject(hdc, gdiFont);

            uint count = GDI.GetKerningPairsW(hdc, (uint)0, null);

            if (count > 0)
            {
                KERNINGPAIR[] kpairs = new KERNINGPAIR[count];
                uint rcount = GDI.GetKerningPairsW(hdc, count, kpairs);
                if (rcount == count)
                {
                    foreach (KERNINGPAIR kp in kpairs)
                    {
                        int idx_first = table[kp.wFirst];
                        int idx_second = table[kp.wSecond];
                        if ((idx_first != NoCode) && (idx_second != NoCode))
                        {
                            kernings[idx_first, idx_second] = (short)kp.iKernAmount;
                        }

                    }
                }
            }

            GDI.SelectObject(hdc, gdiPrevFont);
            GDI.DeleteObject(gdiFont);
            gr.ReleaseHdc(hdc);
            //   subfonts[i].kernings = kernings;
        }

        public Encoding unicode = Encoding.BigEndianUnicode;

        public ushort GetCode(char c)
        {

            byte[] code_in_bytes = unicode.GetBytes(new char[] { c });
            return (ushort)((code_in_bytes[0] << 8) | (code_in_bytes[1] << 0));
        }
        public ushort NoCode
        {
            get
            {
                return 0xFFFF;
            }
        }
        const string charsetNumber = "0123456789!\"#$%&'()*+,-./:;<=>?@[\\]^_`~{}|° ";
        const string charsetLatin = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        const string charsetRussian = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        private ushort[] BuildTable(string charset)
        {
            var table = new ushort[65536];
            for (int i = 0; i < table.Length; i++) table[i] = NoCode;
            for (int i = 0; i < charset.Length; i++)
            {
                ushort code = GetCode(charset[i]);
                table[code] = 0;
            }

            ushort idx = 0;
            for (int i = 0; i < table.Length; i++) if (table[i] != NoCode)
                {
                    table[i] = idx;
                    idx++;
                }
            return table;
        }

        public short GetKerning(char c1, char c2)
        {
            ushort index = table[GetCode(c1)];
            ushort index2 = table[GetCode(c2)];
            if (index != NoCode && index2 != NoCode)
            {
                return kernings[index, index2];
            }
            return 0;
        }
        private Font font;
        private Graphics gr;

        public void GetCharBitmap(char c, Bitmap bmp)
        {
            var r = GetStringRegions(c + "");
            {
                using (Graphics gr2 = Graphics.FromImage(bmp))
                {
                    gr2.Clear(Color.White);
                    gr2.DrawString(c + "", font, Brushes.Black, r[0].DrawPoint.X - r[0].Bound.Left, r[0].DrawPoint.Y);
                    for (int i = 0; i < bmp.Width; i++)
                    {
                        for (int j = 0; j < bmp.Height; j++)
                        {
                            var p = bmp.GetPixel(i, j);
                            if (p.R > 128)
                            {
                                bmp.SetPixel(i, j, Color.FromArgb(0, Color.Black));
                            }
                        }
                    }
                }
            }
        }

        Dictionary<char, CharRegion> Cache = new Dictionary<char, CharRegion>();
        public CharRegion[] GetStringRegionsCached(string text, bool useKerning = true)
        {

            List<CharRegion> ret = new List<CharRegion>();
            CharacterRange[] characterRanges = { new CharacterRange(0, 1) };

            StringFormat stringFormat = new StringFormat();
            stringFormat.FormatFlags = StringFormatFlags.NoClip;
            stringFormat.SetMeasurableCharacterRanges(characterRanges);


            for (int i = 0; i < text.Length; i++)
            {
                var t = text[i];
                if (Cache.ContainsKey(t)) continue;
                var tt = gr.MeasureCharacterRanges(t + "", font, new RectangleF(0, 0, 1000, 1000), stringFormat);
                var ms = gr.MeasureString(t + "", font, 1000, stringFormat);
                //var ms=TextRenderer.MeasureText(t + "", font, new Size(int.MaxValue, int.MaxValue),(TextFormatFlags)( tt++));
                //DrawALineOfText(gr);
                var b = Rectangle.Round(tt[0].GetBounds(gr));
                if (b.Width == 0)
                {
                    b = new Rectangle(b.Left, b.Top, (int)ms.Width, (int)ms.Height);
                }

                var cr = (new CharRegion()
                {
                    DrawPoint = new PointF(0, 0),
                    Bound = new RectangleF(b.Left, 0, b.Width, b.Height)
                });

                Cache.Add(t, cr);

            }
            float pos = 0;
            for (int i = 0; i < text.Length; i++)
            {
                var t = text[i];

                if (i > 0)
                {
                    ushort index = table[GetCode(t)];
                    ushort index2 = table[GetCode(text[i - 1])];
                    if (index != NoCode && index2 != NoCode)
                    {
                        if (useKerning)
                        {
                            pos += kernings[index, index2];
                        }
                    }
                }

                var cr = Cache[t];
                var b = cr.Bound;
                ret.Add(new CharRegion()
                {
                    DrawPoint = new PointF(pos, 0),
                    Bound = new RectangleF(b.Left + pos, b.Top, b.Width, b.Height)
                });


                pos += cr.Bound.Width;

            }
            return ret.ToArray();
        }

        public CharRegion[] GetStringRegions(string text, bool useKerning = true)
        {
            return GetStringRegionsCached(text, useKerning);            
        }
    }
}