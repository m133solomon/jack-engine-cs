using System;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;

namespace Jack.Graphics
{
    public class SpriteFont : IDisposable
    {
        public int GlyphWidth => 11;
        public int GlyphHeight => 22;

        public int GlyphsPerLine => 16;
        public int GlyphLineCount => 16;

        public string FontName { get; }
        public int FontSize { get; }
        public int CharSpacing { get; set; }

        public Texture FontTexture { get; }

        private bool _bitmapFont = false;

        // todo: make a constructor that loads font from file
        public SpriteFont(string fontName, int size)
        {
            FontName = fontName;
            FontSize = size;
            CharSpacing = (int)(FontSize * 0.2f);

            FontTexture = new Texture(MakeFontTexture());
        }

        private MemoryStream MakeFontTexture()
        {
            int bmpWidth = GlyphsPerLine * (GlyphWidth + FontSize);
            int bmpHeight = GlyphLineCount * (GlyphHeight + FontSize);

            using (Bitmap bmp = new Bitmap(bmpWidth, bmpHeight, PixelFormat.Format32bppArgb))
            using (Font font = new Font(new FontFamily(FontName), FontSize))
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bmp))
            {
                if (_bitmapFont)
                {
                    graphics.SmoothingMode = SmoothingMode.None;
                    graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                }
                else
                {
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                }

                for (int i = 0; i < GlyphLineCount; i++)
                {
                    for (int j = 0; j < GlyphsPerLine; j++)
                    {
                        char c = (char)(j + i * GlyphsPerLine);
                        graphics.DrawString(c.ToString(), font, Brushes.White, j * (GlyphWidth + FontSize), i * (GlyphHeight + FontSize));
                    }
                }

                MemoryStream imageStream = new MemoryStream();

                bmp.Save(imageStream, ImageFormat.Png);

                return imageStream;
            }
        }

        public void Dispose()
        {
            FontTexture.Dispose();
        }
    }
}
