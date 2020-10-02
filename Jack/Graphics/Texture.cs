using System;
using System.Runtime.InteropServices;
using OpenTK;
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
        
        public System.Drawing.Size Size { get; }

        public Texture(string path)
        {
            Image<Rgba32> image = (Image<Rgba32>)Image.Load(path);
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            Size = new System.Drawing.Size(image.Width, image.Height);
            
            unsafe
            {
                fixed (void* data = &MemoryMarshal.GetReference(image.GetPixelRowSpan(0)))
                {
                    Load(data, image.Width, image.Height);
                }
            }
        }
        
        public Texture(Span<byte> data, int width, int height)
        {
            unsafe
            {
                fixed (void* d = &data[0])
                {
                    Load(d, width, height);
                }
            }
        }
        
        public unsafe Texture(void *data, int width, int height)
        {
            Load(data, width, height);
        }

        private unsafe void Load(void* data, int width, int height)
        {
            _id = GL.GenTexture();
            Bind();
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            
            // todo: find a way to change linear to neareast
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
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