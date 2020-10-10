using System;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Jack.Graphics
{
    public class Texture : IDisposable
    {
        private int _id;
        public int Id => _id;

        public int Width { get; private set; }
        public int Height { get; private set; }

        // todo: check min / mag filter
        private TextureMinFilter _minFilter;
        private TextureMagFilter _magFilter;

        public System.Drawing.Rectangle Rectangle => new System.Drawing.Rectangle(0, 0, Width, Height);

        public Texture(string path, TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear)
        {
            if (!File.Exists(path))
            {
                throw new FileLoadException("Texture not found: " + path);
            }

            Image<Rgba32> image = (Image<Rgba32>)Image.Load(path);

            _minFilter = minFilter;
            _magFilter = magFilter;

            MakeTexture(image);
        }

        public Texture(MemoryStream stream, TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear)
        {
            Image<Rgba32> image = (Image<Rgba32>)Image.Load(stream.ToArray());

            _minFilter = minFilter;
            _magFilter = magFilter;

            MakeTexture(image);
        }

        public Texture(IntPtr data, int width, int height, TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear)
        {
            Width = width;
            Height = height;

            _minFilter = minFilter;
            _magFilter = magFilter;

            Load(data, width, height);
        }

        private void MakeTexture(Image<Rgba32> image)
        {
            // note: this apply this based on ortho projection
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            Width = image.Width;
            Height = image.Height;

            unsafe
            {
                fixed (void* data = &MemoryMarshal.GetReference(image.GetPixelRowSpan(0)))
                {
                    Load((IntPtr)data, image.Width, image.Height);
                }
            }
        }

        private void Load(IntPtr data, int width, int height)
        {
            _id = GL.GenTexture();
            Bind();

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)_minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)_magFilter);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            Unbind();
        }

        public void Bind(TextureUnit slot = TextureUnit.Texture0)
        {
            GL.ActiveTexture(slot);
            GL.BindTexture(TextureTarget.Texture2D, _id);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            GL.DeleteTexture(_id);
        }
    }
}