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
        // todo: find a way to optimize bitmap size
        // + general font spacing
        public float GlyphWidth { get; private set; }
        public float GlyphHeight { get; private set; }

        public int GlyphsPerLine => 16;
        public int GlyphLineCount => 16;

        public string FontName { get; }
        public int FontSize { get; }
        public int CharSpacing { get; set; }
        public int TextureCharSpacing => 10;

        public Texture FontTexture { get; }

        private bool _bitmapFont = false;

        // todo: make a constructor that loads font from file
        public SpriteFont(string fontName, int size)
        {
            FontName = fontName;
            FontSize = size;

            FontTexture = new Texture(MakeFontTexture());
        }

        private System.Drawing.Graphics _graphics;
        private Font _font;

        private MemoryStream MakeFontTexture()
        {
            _font = new Font(new FontFamily(FontName), FontSize);
            // note: maybe I can find a way to eaily fit all chars on a smaller bitmap
            int bmpWidth = (int)(GlyphsPerLine * (FontSize * 2f));
            int bmpHeight = (int)(GlyphLineCount * (FontSize * 2f));

            using (Bitmap bmp = new Bitmap(bmpWidth, bmpHeight, PixelFormat.Format32bppArgb))
            {
                _graphics = System.Drawing.Graphics.FromImage(bmp);
                if (_bitmapFont)
                {
                    _graphics.SmoothingMode = SmoothingMode.None;
                    _graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                }
                else
                {
                    _graphics.SmoothingMode = SmoothingMode.HighQuality;
                    _graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                }

                SizeF glyphSize = _graphics.MeasureString("A", _font);
                // note: maybe find a better ratio for the spacing
                CharSpacing = (int)(_graphics.MeasureString(" ", _font).Width * 0.6f);

                GlyphWidth = glyphSize.Width;
                GlyphHeight = glyphSize.Height;

                for (int i = 0; i < GlyphLineCount; i++)
                {
                    for (int j = 0; j < GlyphsPerLine; j++)
                    {
                        char c = (char)(j + i * GlyphsPerLine);
                        int x = (int)(j * (GlyphWidth + TextureCharSpacing));
                        int y = (int)(i * (GlyphHeight + TextureCharSpacing));
                        _graphics.DrawString(c.ToString(), _font, Brushes.White, x, y);
                    }
                }

                MemoryStream imageStream = new MemoryStream();
                bmp.Save(imageStream, ImageFormat.Png);
                return imageStream;
            }
        }

        public Rectangle GetBounds(string text)
        {
            SizeF textSize = _graphics.MeasureString(text, _font);
            return new Rectangle(0, 0, (int)textSize.Width + CharSpacing * text.Length / 5, (int)textSize.Height);
        }

        public void Dispose()
        {
            FontTexture.Dispose();
            _graphics.Dispose();
            _font.Dispose();
        }
    }
}
